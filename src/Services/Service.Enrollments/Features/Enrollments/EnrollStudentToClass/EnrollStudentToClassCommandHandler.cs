using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommandHandler : IRequestHandler<EnrollStudentToClassCommand, ErrorOr<Created>>
{
  private const int MaxConcurrencyAttempts = 3;

  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<EnrollStudentToClassCommandHandler> _logger;


  public EnrollStudentToClassCommandHandler(ILogger<EnrollStudentToClassCommandHandler> logger,
    ApplicationDbContext dbContext
  )
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Created>> Handle(EnrollStudentToClassCommand command,
    CancellationToken cancellationToken)
  {
    var alreadyProcessed = await _dbContext.IdempotencyRecords
      .AnyAsync(r => r.IdempotencyKey == command.IdempotencyKey, cancellationToken);
    if (alreadyProcessed)
    {
      _logger.LogInformation(
        "Enrollment request with idempotency key {IdempotencyKey} was already processed, returning success",
        command.IdempotencyKey);
      return Result.Created;
    }

    for (var attempt = 0; attempt < MaxConcurrencyAttempts; attempt++)
    {
      var existingEnrollment = await _dbContext.Enrollments
        .FirstOrDefaultAsync(e =>
            e.CourseId == command.CourseId
            && e.StudentId == command.StudentId
            && e.ClassId == command.ClassId
          , cancellationToken);
      if (existingEnrollment != null)
      {
        _logger.LogWarning("Student {StudentId} is already enrolled to course {CourseId}", command.StudentId,
          command.CourseId);
        return Error.Conflict("enrollment_service.enroll_student_to_course.already_enrolled",
          $"Student {command.StudentId} is already enrolled to course {command.CourseId}");
      }

      var existingClass = await _dbContext.Classes.FirstOrDefaultAsync(x => x.Id == command.ClassId, cancellationToken);
      if (existingClass == null)
      {
        _logger.LogWarning("Class with id {ClassId} not found for course {CourseId}", command.ClassId,
          command.CourseId);
        return Error.NotFound("enrollment_service.enroll_student_to_course.class_not_found",
          $"Class with id {command.ClassId} not found for course {command.CourseId}");
      }

      if (existingClass.RegistrationDeadline < DateTime.UtcNow)
      {
        _logger.LogWarning("Class with id {ClassId} registration deadline has passed for course {CourseId}",
          command.ClassId, command.CourseId);
        return Error.Conflict("enrollment_service.enroll_student_to_course.registration_deadline_passed",
          $"Class with id {command.ClassId} registration deadline has passed for course {command.CourseId}");
      }

      if (existingClass.EnrolledCount >= existingClass.MaxStudents)
      {
        _logger.LogWarning("Class with id {ClassId} is full for course {CourseId}", command.ClassId,
          command.CourseId);
        return Error.Conflict("enrollment_service.enroll_student_to_course.class_full",
          $"Class with id {command.ClassId} is full for course {command.CourseId}");
      }

      existingClass.EnrolledCount += 1;

      var enrollment = new Enrollment
      {
        CourseId = command.CourseId,
        ClassId = command.ClassId,
        StudentId = command.StudentId,
        StudentFirstName = command.FirstName,
        StudentLastName = command.LastName
      };
      await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);

      await _dbContext.IdempotencyRecords.AddAsync(
        new IdempotencyRecord { IdempotencyKey = command.IdempotencyKey, Operation = "Enroll" },
        cancellationToken);

      try
      {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Created;
      }
      catch (DbUpdateConcurrencyException)
      {
        _logger.LogInformation(
          "Concurrent enrollment change detected for class {ClassId}, retrying (attempt {Attempt})",
          command.ClassId, attempt + 1);
        _dbContext.ChangeTracker.Clear();
      }
    }

    _logger.LogWarning("Too many concurrent enrollment attempts for class {ClassId}", command.ClassId);
    return Error.Conflict("enrollment_service.enroll_student_to_course.concurrency_conflict",
      "Too many concurrent enrollment attempts, please retry.");
  }
}
