using Library.Database;
using Library.Dates;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Configurations;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

/// <summary>
/// Enrolls a student in a class.
/// <list type="bullet">
/// <item>The same request (same idempotency key) is applied only once.</item>
/// <item>A student can be in a class only once.</item>
/// <item>Registration must still be open, and the class must have a free seat.</item>
/// <item>The seat is taken with one atomic SQL UPDATE, so two students can never get the last seat.</item>
/// </list>
/// Everything happens in one transaction: either the student is enrolled and the seat is taken, or nothing changes.
/// </summary>
public class EnrollStudentToClassCommandHandler : IRequestHandler<EnrollStudentToClassCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<EnrollStudentToClassCommandHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public EnrollStudentToClassCommandHandler(ILogger<EnrollStudentToClassCommandHandler> logger,
    ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Created>> Handle(EnrollStudentToClassCommand command,
    CancellationToken cancellationToken)
  {
    if (await WasAlreadyHandled(command.IdempotencyKey, cancellationToken))
    {
      _logger.LogInformation("Enroll request {IdempotencyKey} was already handled", command.IdempotencyKey);
      return Result.Created;
    }

    var now = _timeProvider.GetUtcNow().UtcDateTime;
    await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    // Step 1: record the request key BEFORE any other check. If the same request runs in parallel, the copy
    // that comes second waits here until the first one finishes, then stops and correctly reports success.
    // (Checking "already enrolled" first would make that second copy wrongly report "already enrolled".)
    if (!await TryRecordRequestKey(command.IdempotencyKey, now, cancellationToken))
    {
      _logger.LogInformation("Enroll request {IdempotencyKey} was handled concurrently", command.IdempotencyKey);
      return Result.Created;
    }

    // Leaving without Commit (any return below) rolls back everything, including step 1.
    if (await IsAlreadyEnrolled(command, cancellationToken))
    {
      _logger.LogWarning("Student {StudentId} is already in class {ClassId}", command.StudentId, command.ClassId);
      return EnrollmentErrors.AlreadyEnrolled(command.StudentId, command.ClassId);
    }

    // Step 2: take a seat.
    var seatResult = await TakeSeat(command, now, cancellationToken);
    if (seatResult.IsError)
    {
      return seatResult.Errors;
    }

    // Step 3: save the enrollment.
    _dbContext.Enrollments.Add(command.ToEnrollment(now));

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception)
      when (exception.IsUniqueViolation(EnrollmentConfiguration.OneEnrollmentPerStudentAndClassIndex))
    {
      // The same student enrolled in parallel with a different request key.
      _logger.LogWarning("Student {StudentId} enrolled concurrently in class {ClassId}", command.StudentId,
        command.ClassId);
      return EnrollmentErrors.AlreadyEnrolled(command.StudentId, command.ClassId);
    }

    await transaction.CommitAsync(cancellationToken);
    _logger.LogInformation("Student {StudentId} enrolled in class {ClassId}", command.StudentId, command.ClassId);
    return Result.Created;
  }

  private Task<bool> WasAlreadyHandled(Guid idempotencyKey, CancellationToken cancellationToken) =>
    _dbContext.IdempotencyRecords.AnyAsync(record => record.IdempotencyKey == idempotencyKey, cancellationToken);

  /// <returns>false when the same key was stored by a request that ran at the same time.</returns>
  private async Task<bool> TryRecordRequestKey(Guid idempotencyKey, DateTime now, CancellationToken cancellationToken)
  {
    _dbContext.IdempotencyRecords.Add(new IdempotencyRecord
    {
      IdempotencyKey = idempotencyKey, Operation = IdempotencyRecord.EnrollOperation, CreatedAt = now
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

  private Task<bool> IsAlreadyEnrolled(EnrollStudentToClassCommand command, CancellationToken cancellationToken) =>
    _dbContext.Enrollments.AnyAsync(
      enrollment => enrollment.StudentId == command.StudentId && enrollment.ClassId == command.ClassId,
      cancellationToken);

  /// <summary>
  /// Adds one to the class's enrolled count, but only if registration is open and a seat is free.
  /// The check and the change happen in the same SQL statement, so parallel requests cannot overbook.
  /// </summary>
  private async Task<ErrorOr<Success>> TakeSeat(EnrollStudentToClassCommand command, DateTime now,
    CancellationToken cancellationToken)
  {
    var changedRows = await _dbContext.Classes
      .Where(c => c.Id == command.ClassId
                  && c.CourseId == command.CourseId
                  && c.RegistrationDeadline >= now
                  && c.EnrolledCount < c.MaxStudents)
      .ExecuteUpdateAsync(setters => setters
        .SetProperty(c => c.EnrolledCount, c => c.EnrolledCount + 1)
        .SetProperty(c => c.UpdatedAt, now), cancellationToken);

    if (changedRows == 1)
    {
      return Result.Success;
    }

    // Nothing changed. Read the class once to tell the caller why.
    var courseClass = await _dbContext.Classes
      .AsNoTracking()
      .FirstOrDefaultAsync(c => c.Id == command.ClassId && c.CourseId == command.CourseId, cancellationToken);

    if (courseClass is null)
    {
      _logger.LogWarning("Class {ClassId} of course {CourseId} was not found", command.ClassId, command.CourseId);
      return EnrollmentErrors.ClassNotFound(command.ClassId, command.CourseId);
    }

    if (courseClass.RegistrationDeadline.AsUtc() < now)
    {
      _logger.LogWarning("Registration for class {ClassId} is closed", command.ClassId);
      return EnrollmentErrors.RegistrationClosed(command.ClassId);
    }

    _logger.LogWarning("Class {ClassId} is full", command.ClassId);
    return EnrollmentErrors.ClassFull(command.ClassId);
  }
}
