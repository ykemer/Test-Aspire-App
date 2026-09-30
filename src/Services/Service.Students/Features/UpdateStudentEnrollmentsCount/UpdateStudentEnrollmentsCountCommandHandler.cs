using Library.Database;

using Service.Students.Common.Database;
using Service.Students.Common.Database.Configurations;
using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

/// <summary>
/// Adds or removes one enrollment on a student's counter.
/// <list type="bullet">
/// <item>The same message is applied only once (inbox table), even if the broker delivers it twice.</item>
/// <item>The counter never goes below zero.</item>
/// <item>The counter is changed with one atomic SQL UPDATE, so parallel messages cannot overwrite each other.</item>
/// </list>
/// Both steps happen in one transaction: either the message is recorded and the counter changes, or nothing changes.
/// </summary>
public class UpdateStudentEnrollmentsCountCommandHandler
  : IRequestHandler<UpdateStudentEnrollmentsCountCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateStudentEnrollmentsCountCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UpdateStudentEnrollmentsCountCommandHandler(ILogger<UpdateStudentEnrollmentsCountCommandHandler> logger,
    ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateStudentEnrollmentsCountCommand request,
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

    var changeResult = await ChangeCount(request, now, cancellationToken);
    if (changeResult.IsError)
    {
      return changeResult.Errors; // leaving without Commit rolls everything back
    }

    await transaction.CommitAsync(cancellationToken);
    _logger.LogInformation("Enrollments count of student {StudentId} changed: {Change}", request.StudentId,
      request.Change);
    return Result.Updated;
  }

  /// <returns>false when this message is already in the inbox, meaning it was handled before.</returns>
  private async Task<bool> TryAddToInbox(UpdateStudentEnrollmentsCountCommand request, DateTime now,
    CancellationToken cancellationToken)
  {
    _dbContext.InboxMessages.Add(new InboxMessage
    {
      MessageId = request.EventId, MessageType = $"StudentEnrollmentsCount.{request.Change}", ProcessedAt = now
    });

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (DbUpdateException exception) when (exception.IsUniqueViolation(InboxMessageConfiguration.PrimaryKeyName))
    {
      return false;
    }
  }

  private async Task<ErrorOr<Updated>> ChangeCount(UpdateStudentEnrollmentsCountCommand request, DateTime now,
    CancellationToken cancellationToken)
  {
    var student = _dbContext.Students.Where(s => s.Id == request.StudentId);
    var delta = 1;

    if (request.Change == EnrollmentChange.RemoveEnrollment)
    {
      student = student.Where(s => s.EnrollmentsCount > 0);
      delta = -1;
    }

    var changedRows = await student.ExecuteUpdateAsync(setters => setters
      .SetProperty(s => s.EnrollmentsCount, s => s.EnrollmentsCount + delta)
      .SetProperty(s => s.UpdatedAt, now), cancellationToken);

    if (changedRows == 1)
    {
      return Result.Updated;
    }

    var studentExists = await _dbContext.Students.AnyAsync(s => s.Id == request.StudentId, cancellationToken);
    _logger.LogWarning("Cannot change enrollments count of student {StudentId}. Student exists: {StudentExists}",
      request.StudentId, studentExists);
    return studentExists
      ? StudentErrors.HasNoEnrollmentsToRemove(request.StudentId)
      : StudentErrors.NotFound(request.StudentId);
  }
}
