using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using MassTransit;

using Service.Enrollments.Common.Database;

namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

public class DeleteEnrollmentsByCourseCommandHandler : IRequestHandler<DeleteEnrollmentsByCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteEnrollmentsByCourseCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteEnrollmentsByCourseCommandHandler(ApplicationDbContext dbContext,
    ILogger<DeleteEnrollmentsByCourseCommandHandler> logger, IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteEnrollmentsByCourseCommand request,
    CancellationToken cancellationToken)
  {
    var events = await _dbContext.Enrollments
      .Where(enrollment => enrollment.CourseId == request.CourseId)
      .Select(enrollment =>
        new DecreaseStudentEnrollmentCountEvent { StudentId = enrollment.StudentId })
      .ToListAsync(cancellationToken);


    _dbContext.RemoveRange(_dbContext.Enrollments.Where(enrollment => enrollment.CourseId == request.CourseId));
    _dbContext.RemoveRange(_dbContext.Classes.Where(course => course.CourseId == request.CourseId));

    await _dbContext.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Deleting enrollments for course {CourseId}", request.CourseId);
    if (events.Count > 0)
    {
      await _publishEndpoint.PublishBatch(events, cancellationToken);
    }

    return Result.Deleted;
  }
}
