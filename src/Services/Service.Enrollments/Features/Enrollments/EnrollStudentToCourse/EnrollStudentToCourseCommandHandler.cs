using Contracts.Enrollments.Events;

using MassTransit;

using Service.Enrollments.Entities;


namespace Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;

public class EnrollStudentToCourseCommandHandler : IRequestHandler<EnrollStudentToCourseCommand, ErrorOr<Created>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<EnrollStudentToCourseCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;


  public EnrollStudentToCourseCommandHandler(ILogger<EnrollStudentToCourseCommandHandler> logger,
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint)
  {
    _logger = logger;
    _dbContext = dbContext;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Created>> Handle(EnrollStudentToCourseCommand command, CancellationToken cancellationToken)
  {
    var existingEnrollment = await _dbContext.Enrollments
      .FirstOrDefaultAsync(e => e.CourseId == command.CourseId && e.StudentId == command.StudentId, cancellationToken);
    if (existingEnrollment != null)
    {
      _logger.LogWarning("Student {StudentId} is already enrolled to course {CourseId}", command.StudentId,
        command.CourseId);
      return Error.Conflict("enrollment_service.enroll_student_to_course.already_enrolled",
        $"Student {command.StudentId} is already enrolled to course {command.CourseId}");
    }

    var enrollment = new Enrollment()
    {
      CourseId = command.CourseId,
      StudentId = command.StudentId,
      StudentFirstName = command.FirstName,
      StudentLastName = command.LastName
    };
    await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);

    return Result.Created;
  }
}
