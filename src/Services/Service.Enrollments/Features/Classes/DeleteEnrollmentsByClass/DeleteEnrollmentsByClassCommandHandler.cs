using Contracts.Students.Events.ChangeStudentEnrollmentsCount;

using MassTransit;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

namespace Service.Enrollments.Features.Classes.DeleteEnrollmentsByClass;

public class DeleteEnrollmentsByClassCommandHandler : IRequestHandler<DeleteEnrollmentsByClassCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByCourseCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteEnrollmentsByClassCommandHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByCourseCommandHandler> logger, IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByClassCommand request,
    CancellationToken cancellationToken)
  {
    var events = await _dbContext.Enrollments
      .Where(enrollment => enrollment.CourseId == request.CourseId && enrollment.ClassId == request.ClassId)
      .Select(enrollment =>
        new ChangeStudentEnrollmentsCountEvent { StudentId = enrollment.StudentId, IsIncrease = false })
      .ToListAsync(cancellationToken);


    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment =>
      enrollment.CourseId == request.CourseId && enrollment.ClassId == request.ClassId));
    _dbContext.RemoveRange(_dbContext.Classes.Where(course => course.Id == request.ClassId));

    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Deleting enrollments for course {CourseId} and class {ClassId}", request.CourseId,
      request.ClassId);
    if (events.Count > 0)
    {
      await _publishEndpoint.PublishBatch(events, cancellationToken);
    }

    return Result.Deleted;
  }
}
