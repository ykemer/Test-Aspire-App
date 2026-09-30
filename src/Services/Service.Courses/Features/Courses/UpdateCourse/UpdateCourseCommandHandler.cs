using Library.Database;

using Service.Courses.Common.Database;

namespace Service.Courses.Features.Courses.UpdateCourse;

/// <summary>
/// Changes the name and description of a course. The new name must not belong to another course.
/// </summary>
public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateCourseCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UpdateCourseCommandHandler(ApplicationDbContext dbContext, ILogger<UpdateCourseCommandHandler> logger,
    TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _logger = logger;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    var course = await _dbContext.Courses.FindAsync([request.Id], cancellationToken);
    if (course is null)
    {
      _logger.LogWarning("Cannot update course {CourseId} because it was not found", request.Id);
      return CourseErrors.NotFound(request.Id);
    }

    var lowerCaseName = request.Name.ToLowerInvariant();
    var nameIsTakenByAnotherCourse = await _dbContext.Courses
      .AnyAsync(other => other.Id != request.Id && other.Name.ToLower() == lowerCaseName, cancellationToken);
    if (nameIsTakenByAnotherCourse)
    {
      _logger.LogWarning("Cannot rename course {CourseId} to {CourseName}: name is taken", request.Id, request.Name);
      return CourseErrors.NameAlreadyTaken(request.Name);
    }

    course.ApplyUpdate(request, _timeProvider.GetUtcNow().UtcDateTime);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      _logger.LogWarning("Course {CourseId} was modified concurrently", request.Id);
      return CourseErrors.ModifiedConcurrently(request.Id);
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation())
    {
      _logger.LogWarning("Course name {CourseName} was taken concurrently", request.Name);
      return CourseErrors.NameAlreadyTaken(request.Name);
    }

    _logger.LogInformation("Course {CourseId} was updated", request.Id);
    return Result.Updated;
  }
}
