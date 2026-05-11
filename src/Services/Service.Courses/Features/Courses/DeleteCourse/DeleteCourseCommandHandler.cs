using Contracts.Courses.Events;

using Rebus.Bus;

using Service.Courses.Common.Database;

namespace Service.Courses.Features.Courses.DeleteCourse;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteCourseCommandHandler> _logger;
  private readonly IBus _bus;

  public DeleteCourseCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteCourseCommandHandler> logger,
    IBus bus)
  {
    _dbContext = dbContext;
    _logger = logger;
    _bus = bus;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
  {
    var course = await _dbContext.Courses.FirstOrDefaultAsync(course => course.Id == request.Id, cancellationToken);
    if (course == null)
    {
      _logger.Log(LogLevel.Warning, "Course with id {RequestId} was not found", request.Id);
      return Error.Unexpected("courses_service.delete_course.course_not_found",
        $"Course with id {request.Id} not found");
    }

    if (course.TotalStudents > 0)
    {
      _logger.Log(LogLevel.Warning, "Course with id {RequestId} can not be deleted because it has students enrolled",
        request.Id);
      return Error.Conflict("courses_service.delete_course.course_has_students",
        $"Course with id {request.Id} can not be deleted because it has students enrolled");
    }


    _dbContext.Classes.RemoveRange(_dbContext.Classes.Where(c => c.CourseId == course.Id));
    _dbContext.Courses.Remove(course);
    await _dbContext.SaveChangesAsync(cancellationToken);
    await _bus.Publish(new CourseDeletedEvent { CourseId = course.Id });
    _logger.Log(LogLevel.Information, "Course with id {RequestId} was deleted", request.Id);
    return Result.Deleted;
  }
}
