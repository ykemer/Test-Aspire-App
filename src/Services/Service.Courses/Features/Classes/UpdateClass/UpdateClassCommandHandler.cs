using Service.Courses.Common.Database;

namespace Service.Courses.Features.Classes.UpdateClass;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateClassCommandHandler> _logger;

  public UpdateClassCommandHandler(ApplicationDbContext dbContext, ILogger<UpdateClassCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
  {
    var courseClass = await _dbContext.Classes.FirstOrDefaultAsync(
      courseClass => courseClass.Id == request.Id && courseClass.CourseId == request.CourseId, cancellationToken);
    if (courseClass is null)
    {
      _logger.LogError("Can not update class with id {ClassId} for course {CourseId} not found", request.Id,
        request.CourseId);
      return Error.NotFound("course_service.update_class.not_found", $"Course {request.Id} not found");
    }


    if (courseClass.TotalStudents > request.MaxStudents)
    {
      _logger.LogError(
        "Can not update class with id {ClassId} for course {CourseId} because it has more students than the new max students",
        request.Id, request.CourseId);
      return Error.Validation("course_service.update_class.max_students_exceeded",
        $"Class {request.Id} for course {request.CourseId} has more students than the new max students");
    }


    courseClass.AddCommandValues(request);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
