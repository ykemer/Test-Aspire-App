using Service.Courses.Common.Database;

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

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      _logger.LogWarning("Course with id {CourseId} was modified concurrently", request.Id);
      return Error.Conflict("course_service.update_course.concurrent_modification",
        $"Course {request.Id} was modified by someone else, please reload and try again.");
    }

    return Result.Updated;
  }
}
