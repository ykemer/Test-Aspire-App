using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommandHandler : IRequestHandler<EnrollStudentToClassCommand, ErrorOr<Created>>
{
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

    var existingClass = _dbContext.Classes.Include(x => x.Enrollments).FirstOrDefault(x => x.Id == command.ClassId);
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

    if (existingClass.MaxStudents  < existingClass.Enrollments.Count + 1)
    {
      _logger.LogWarning("Class with id {ClassId} is full for course {CourseId}", command.ClassId,
        command.CourseId);
      return Error.Conflict("enrollment_service.enroll_student_to_course.class_full",
        $"Class with id {command.ClassId} is full for course {command.CourseId}");
    }

    var enrollment = new Enrollment()
    {
      CourseId = command.CourseId,
      ClassId = command.ClassId,
      StudentId = command.StudentId,
      StudentFirstName = command.FirstName,
      StudentLastName = command.LastName
    };


    await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    return Result.Created;
  }
}
