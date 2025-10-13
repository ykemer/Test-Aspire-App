using Contracts.Courses.Events;

using MassTransit;

using Service.Courses.Common.Database;

namespace Service.Courses.Features.Classes.DeleteClass;

public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, ErrorOr<Deleted>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<DeleteClassCommandHandler> _logger;
  private readonly IPublishEndpoint _publishEndpoint;

  public DeleteClassCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteClassCommandHandler> logger,
    IPublishEndpoint publishEndpoint)
  {
    _dbContext = dbContext;
    _logger = logger;
    _publishEndpoint = publishEndpoint;
  }

  public async ValueTask<ErrorOr<Deleted>> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext
      .Classes
      .FirstOrDefaultAsync(courseClass => courseClass.Id == request.Id && courseClass.CourseId == request.CourseId, cancellationToken);
    if (courseClass == null)
    {
      _logger.Log(LogLevel.Warning, "Course class with id {RequestId} was not found", request.Id);
      return Error.Unexpected("courses_service.delete_course_class.class_not_found",
        $"Course with id {request.Id} not found");
    }

    if(courseClass.TotalStudents > 0)
    {
      _logger.Log(LogLevel.Warning, "Course class with id {RequestId} can not be deleted because it has students enrolled", request.Id);
      return Error.Conflict("courses_service.delete_course_class.class_has_students",
        $"Course class with id {request.Id} can not be deleted because it has students enrolled");
    }

    _dbContext.Classes.Remove(courseClass);
    await _dbContext.SaveChangesAsync(cancellationToken);
    await _publishEndpoint.Publish((object)new ClassDeletedEvent
    {
      CourseId = courseClass.CourseId,
      ClassId = courseClass.Id,
    }, cancellationToken);
    _logger.Log(LogLevel.Information, "Course with id {RequestId} was deleted", request.Id);
    return Result.Deleted;
  }
}
