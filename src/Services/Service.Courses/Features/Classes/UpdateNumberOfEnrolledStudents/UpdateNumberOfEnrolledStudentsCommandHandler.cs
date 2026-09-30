using Library.Database;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

/// <summary>
/// Adds or removes one enrolled student on a class and on its course.
/// <list type="bullet">
/// <item>The same message is applied only once (inbox table), even if the broker delivers it twice.</item>
/// <item>A class never goes above its maximum, and no counter goes below zero.</item>
/// <item>Each counter is changed with one atomic SQL UPDATE, so parallel messages cannot overwrite each other.</item>
/// </list>
/// Everything happens in one transaction: either both counters change, or nothing changes.
/// </summary>
public class UpdateNumberOfEnrolledStudentsCommandHandler
  : IRequestHandler<UpdateNumberOfEnrolledStudentsCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UpdateNumberOfEnrolledStudentsCommandHandler(ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> logger,
    ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateNumberOfEnrolledStudentsCommand request,
    CancellationToken cancellationToken)
  {
    var now = _timeProvider.GetUtcNow().UtcDateTime;
    await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    var isFirstDelivery = await TryAddToInbox(request, now, cancellationToken);
    if (!isFirstDelivery)
    {
      _logger.LogInformation("Message {EventId} was already handled, skipping it", request.EventId);
      return Result.Updated;
    }

    var courseResult = await ChangeCourseCount(request, now, cancellationToken);
    if (courseResult.IsError)
    {
      return courseResult.Errors; // leaving without Commit rolls everything back
    }

    var classResult = await ChangeClassCount(request, now, cancellationToken);
    if (classResult.IsError)
    {
      return classResult.Errors;
    }

    await transaction.CommitAsync(cancellationToken);
    return Result.Updated;
  }

  /// <returns>false when this message is already in the inbox, meaning it was handled before.</returns>
  private async Task<bool> TryAddToInbox(UpdateNumberOfEnrolledStudentsCommand request, DateTime now,
    CancellationToken cancellationToken)
  {
    _dbContext.InboxMessages.Add(new InboxMessage
    {
      MessageId = request.EventId, MessageType = $"EnrollmentCount.{request.Change}", ProcessedAt = now
    });

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation())
    {
      return false;
    }
  }

  private async Task<ErrorOr<Updated>> ChangeCourseCount(UpdateNumberOfEnrolledStudentsCommand request,
    DateTime now, CancellationToken cancellationToken)
  {
    var course = _dbContext.Courses.Where(course => course.Id == request.CourseId);
    var delta = 1;

    if (request.Change == EnrollmentChange.RemoveStudent)
    {
      course = course.Where(c => c.TotalStudents > 0);
      delta = -1;
    }

    var changedRows = await course.ExecuteUpdateAsync(setters => setters
      .SetProperty(c => c.TotalStudents, c => c.TotalStudents + delta)
      .SetProperty(c => c.UpdatedAt, now), cancellationToken);

    if (changedRows == 1)
    {
      return Result.Updated;
    }

    var courseExists = await _dbContext.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
    _logger.LogWarning("Cannot change student count of course {CourseId}. Course exists: {CourseExists}",
      request.CourseId, courseExists);
    return courseExists
      ? CourseErrors.HasNoStudentsToRemove(request.CourseId)
      : CourseErrors.NotFound(request.CourseId);
  }

  private async Task<ErrorOr<Updated>> ChangeClassCount(UpdateNumberOfEnrolledStudentsCommand request,
    DateTime now, CancellationToken cancellationToken)
  {
    var courseClass = _dbContext.Classes
      .Where(c => c.Id == request.ClassId && c.CourseId == request.CourseId);
    var delta = 1;

    if (request.Change == EnrollmentChange.AddStudent)
    {
      courseClass = courseClass.Where(c => c.TotalStudents < c.MaxStudents);
    }
    else
    {
      courseClass = courseClass.Where(c => c.TotalStudents > 0);
      delta = -1;
    }

    var changedRows = await courseClass.ExecuteUpdateAsync(setters => setters
      .SetProperty(c => c.TotalStudents, c => c.TotalStudents + delta)
      .SetProperty(c => c.UpdatedAt, now), cancellationToken);

    if (changedRows == 1)
    {
      return Result.Updated;
    }

    var classExists = await _dbContext.Classes
      .AnyAsync(c => c.Id == request.ClassId && c.CourseId == request.CourseId, cancellationToken);
    _logger.LogWarning("Cannot change student count of class {ClassId}. Class exists: {ClassExists}",
      request.ClassId, classExists);

    if (!classExists)
    {
      return ClassErrors.NotFound(request.ClassId, request.CourseId);
    }

    return request.Change == EnrollmentChange.AddStudent
      ? ClassErrors.IsFull(request.ClassId)
      : ClassErrors.HasNoStudentsToRemove(request.ClassId);
  }
}
