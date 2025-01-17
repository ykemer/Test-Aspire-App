using Contracts.Courses.Events;

using Service.Courses.AsyncDataServices;
using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.DeleteCourse;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteCourseHandler> _logger;
  private readonly IMessageBusClient _messageBusClient;

  public DeleteCourseHandler(ApplicationDbContext dbContext, ILogger<DeleteCourseHandler> logger,
    IMessageBusClient messageBusClient)
  {
    _dbContext = dbContext;
    _logger = logger;
    _messageBusClient = messageBusClient;
  }

  public async Task<ErrorOr<Deleted>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
  {
    var course = await _dbContext.Courses.FirstOrDefaultAsync(course => course.Id == request.Id, cancellationToken);
    if (course == null)
    {
      _logger.Log(LogLevel.Warning, "Course with id {RequestId} was not found", request.Id);
      return Error.Unexpected("courses_service.delete_course.course_not_found",
        $"Course with id {request.Id} not found");
    }

    _dbContext.Courses.Remove(course);
    await _dbContext.SaveChangesAsync(cancellationToken);
    _messageBusClient.PublishCourseDeletedMessage(new CourseDeletedEvent { CourseId = course.Id });
    _logger.Log(LogLevel.Information, "Course with id {RequestId} was deleted", request.Id);
    return Result.Deleted;
  }
}
