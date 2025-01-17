using Contracts.Enrollments.Events;

using Service.Enrollments.AsyncDataServices;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

public class UnenrollStudentFromCourseHandler : IRequestHandler<UnenrollStudentFromCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UnenrollStudentFromCourseHandler> _logger;
  private readonly IMessageBusClient _messageBusClient;

  public UnenrollStudentFromCourseHandler(ILogger<UnenrollStudentFromCourseHandler> logger,
    ApplicationDbContext dbContext, IMessageBusClient messageBusClient)
  {
    _logger = logger;
    _dbContext = dbContext;
    _messageBusClient = messageBusClient;
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
    _messageBusClient.PublishStudentUnenrolledEvent(new StudentUnenrolledEvent
    {
      StudentId = command.StudentId, CourseId = command.CourseId
    });

    return Result.Deleted;
  }
}
