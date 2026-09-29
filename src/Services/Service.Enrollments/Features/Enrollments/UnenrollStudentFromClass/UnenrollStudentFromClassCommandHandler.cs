using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommandHandler
  : IRequestHandler<UnenrollStudentFromClassCommand, ErrorOr<Deleted>>
{
  private const int MaxConcurrencyAttempts = 3;

  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UnenrollStudentFromClassCommandHandler> _logger;

  public UnenrollStudentFromClassCommandHandler(ILogger<UnenrollStudentFromClassCommandHandler> logger,
    ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(UnenrollStudentFromClassCommand command,
    CancellationToken cancellationToken)
  {
    var alreadyProcessed = await _dbContext.IdempotencyRecords
      .AnyAsync(r => r.IdempotencyKey == command.IdempotencyKey, cancellationToken);
    if (alreadyProcessed)
    {
      _logger.LogInformation(
        "Unenrollment request with idempotency key {IdempotencyKey} was already processed, returning success",
        command.IdempotencyKey);
      return Result.Deleted;
    }

    for (var attempt = 0; attempt < MaxConcurrencyAttempts; attempt++)
    {
      var existingClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == command.ClassId, cancellationToken);
      if (existingClass is null)
      {
        _logger.LogWarning("Class with id {ClassId} doesn't exist", command.ClassId);
        return Error.NotFound("enrollment_service.unenroll_student_from_course.class_not_found",
          $"Class with id {command.ClassId} doesn't exist");
      }

      var existingEnrollment = await _dbContext.Enrollments
        .FirstOrDefaultAsync(e => e.CourseId == command.CourseId
                                  && e.ClassId == command.ClassId
                                  && e.StudentId == command.StudentId, cancellationToken);
      if (existingEnrollment == null)
      {
        _logger.LogWarning("Enrollment for student {StudentId}, course {CourseId} and {ClassId} doesn't exist",
          command.StudentId,
          command.CourseId, command.ClassId);
        return Error.Conflict("enrollment_service.unenroll_student_from_course.not_enrolled",
          $"Student {command.StudentId} is not enrolled to course {command.CourseId}");
      }

      if (existingClass.CourseStartDate <= DateTime.UtcNow)
      {
        _logger.LogWarning("Cannot unenroll student {StudentId} from class {ClassId} as the class has already started",
          command.StudentId, command.ClassId);

        return Error.Conflict("enrollment_service.unenroll_student_from_course.class_already_started",
          $"Cannot unenroll student {command.StudentId} from class {command.ClassId} as the class has already started");
      }

      existingClass.EnrolledCount = Math.Max(0, existingClass.EnrolledCount - 1);

      _dbContext.Enrollments.Remove(existingEnrollment);

      await _dbContext.IdempotencyRecords.AddAsync(
        new IdempotencyRecord { IdempotencyKey = command.IdempotencyKey, Operation = "Unenroll" },
        cancellationToken);

      try
      {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
      }
      catch (DbUpdateConcurrencyException)
      {
        _logger.LogInformation(
          "Concurrent enrollment change detected for class {ClassId}, retrying (attempt {Attempt})",
          command.ClassId, attempt + 1);
        _dbContext.ChangeTracker.Clear();
      }
    }

    _logger.LogWarning("Too many concurrent unenrollment attempts for class {ClassId}", command.ClassId);
    return Error.Conflict("enrollment_service.unenroll_student_from_course.concurrency_conflict",
      "Too many concurrent unenrollment attempts, please retry.");
  }
}
