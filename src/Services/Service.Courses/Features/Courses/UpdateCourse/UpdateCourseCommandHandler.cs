namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateCourseCommandHandler> _logger;

  public UpdateCourseCommandHandler(ApplicationDbContext dbContext, ILogger<UpdateCourseCommandHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    var existingCourse = await _dbContext.Courses.FindAsync(request.Id, cancellationToken);
    if (existingCourse is null)
    {
      _logger.LogError("Can not update course with id {CourseId} not found", request.Id);
      return Error.NotFound("course_service.update_course.course.not_found", $"Course {request.Id} not found");
    }

    existingCourse.AddCommandValues(request);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
