using Library.Database;
using Library.Dates;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Configurations;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

/// <summary>
/// Removes a student from a class and frees their seat.
/// <list type="bullet">
/// <item>The same request (same idempotency key) is applied only once.</item>
/// <item>Students cannot leave a class that has already started.</item>
/// <item>The seat is freed with one atomic SQL UPDATE, so parallel requests cannot miscount.</item>
/// </list>
/// Everything happens in one transaction: either the student is removed and the seat is freed, or nothing changes.
/// </summary>
public class UnenrollStudentFromClassCommandHandler
  : IRequestHandler<UnenrollStudentFromClassCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UnenrollStudentFromClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public UnenrollStudentFromClassCommandHandler(ILogger<UnenrollStudentFromClassCommandHandler> logger,
    ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(UnenrollStudentFromClassCommand command,
    CancellationToken cancellationToken)
  {
    var alreadyHandled = await _dbContext.IdempotencyRecords
      .AnyAsync(record => record.IdempotencyKey == command.IdempotencyKey, cancellationToken);
    if (alreadyHandled)
    {
      _logger.LogInformation("Unenroll request {IdempotencyKey} was already handled", command.IdempotencyKey);
      return Result.Deleted;
    }

    var now = _timeProvider.GetUtcNow().UtcDateTime;
    await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    // Step 1: record the request key BEFORE any other check. If the same request runs in parallel, the copy
    // that comes second waits here until the first one finishes, then stops and correctly reports success.
    // (Looking up the enrollment first would make that second copy wrongly report "not enrolled".)
    if (!await TryRecordRequestKey(command.IdempotencyKey, now, cancellationToken))
    {
      _logger.LogInformation("Unenroll request {IdempotencyKey} was handled concurrently", command.IdempotencyKey);
      return Result.Deleted;
    }

    // Leaving without Commit (any return below) rolls back everything, including step 1.
    var enrollment = await _dbContext.Enrollments
      .Include(e => e.Class)
      .FirstOrDefaultAsync(e => e.CourseId == command.CourseId
                                && e.ClassId == command.ClassId
                                && e.StudentId == command.StudentId, cancellationToken);
    if (enrollment is null)
    {
      _logger.LogWarning("Student {StudentId} is not in class {ClassId}", command.StudentId, command.ClassId);
      return EnrollmentErrors.NotEnrolled(command.StudentId, command.ClassId);
    }

    if (enrollment.Class.CourseStartDate.AsUtc() <= now)
    {
      _logger.LogWarning("Class {ClassId} has already started, student {StudentId} cannot leave", command.ClassId,
        command.StudentId);
      return EnrollmentErrors.ClassAlreadyStarted(command.ClassId);
    }

    // Step 2: remove the enrollment.
    _dbContext.Enrollments.Remove(enrollment);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      // Another request removed this enrollment first (the DELETE found no row).
      _logger.LogWarning("Student {StudentId} was removed from class {ClassId} concurrently", command.StudentId,
        command.ClassId);
      return EnrollmentErrors.NotEnrolled(command.StudentId, command.ClassId);
    }

    // Step 3: free the seat.
    await FreeSeat(command.ClassId, now, cancellationToken);

    await transaction.CommitAsync(cancellationToken);
    _logger.LogInformation("Student {StudentId} left class {ClassId}", command.StudentId, command.ClassId);
    return Result.Deleted;
  }

  /// <returns>false when the same key was stored by a request that ran at the same time.</returns>
  private async Task<bool> TryRecordRequestKey(Guid idempotencyKey, DateTime now, CancellationToken cancellationToken)
  {
    _dbContext.IdempotencyRecords.Add(new IdempotencyRecord
    {
      IdempotencyKey = idempotencyKey, Operation = IdempotencyRecord.UnenrollOperation, CreatedAt = now
    });

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
    }
    catch (DbUpdateException exception)
      when (exception.IsUniqueViolation(IdempotencyRecordConfiguration.PrimaryKeyName))
    {
      return false;
    }
  }

  /// <summary>
  /// Subtracts one from the class's enrolled count in one atomic SQL statement. Never goes below zero.
  /// </summary>
  private async Task FreeSeat(Guid classId, DateTime now, CancellationToken cancellationToken)
  {
    var changedRows = await _dbContext.Classes
      .Where(c => c.Id == classId && c.EnrolledCount > 0)
      .ExecuteUpdateAsync(setters => setters
        .SetProperty(c => c.EnrolledCount, c => c.EnrolledCount - 1)
        .SetProperty(c => c.UpdatedAt, now), cancellationToken);

    if (changedRows == 0)
    {
      // The enrollment existed but the counter was already zero: the counter was wrong before this request.
      // The student is still removed (the enrollment row is the truth), but someone should look at this.
      _logger.LogError("Enrolled count of class {ClassId} was already zero while removing a student", classId);
    }
  }
}
