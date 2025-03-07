using Contracts.Enrollments.Events;

using MassTransit;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

public class UnenrollStudentFromCourseCommandHandler
  : IRequestHandler<UnenrollStudentFromCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UnenrollStudentFromCourseCommandHandler> _logger;

  public UnenrollStudentFromCourseCommandHandler(ILogger<UnenrollStudentFromCourseCommandHandler> logger,
    ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<ErrorOr<Deleted>> Handle(UnenrollStudentFromCourseCommand command,
    CancellationToken cancellationToken)
  {
    var existingEnrollment = await _dbContext.Enrollments
      .FirstOrDefaultAsync(e => e.CourseId == command.CourseId && e.StudentId == command.StudentId, cancellationToken);
    if (existingEnrollment == null)
    {
      _logger.LogWarning("Enrollment for student {StudentId} and course {CourseId} doesn't exist", command.StudentId,
        command.CourseId);
      return Error.Conflict("enrollment_service.unenroll_student_from_course.not_enrolled",
        $"Student {command.StudentId} is not enrolled to course {command.CourseId}");
    }

    _dbContext.Enrollments.Remove(existingEnrollment);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Deleted;
  }
}
