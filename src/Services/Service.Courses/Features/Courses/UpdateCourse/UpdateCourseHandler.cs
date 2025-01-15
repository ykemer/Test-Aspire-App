using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateCourseHandler> _logger;

  public UpdateCourseHandler(ApplicationDbContext dbContext, ILogger<UpdateCourseHandler> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task<ErrorOr<Updated>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
  {
    Course? existingCourse = await _dbContext.Courses.FindAsync(request.Id, cancellationToken);
    if (existingCourse is null)
    {
      _logger.LogError("Can not update course with id {CourseId} not found", request.Id);
      return Error.NotFound("course_service.update_course.course.not_found", $"Course {request.Id} not found");
    }

    existingCourse.Name = request.Name;
    existingCourse.Description = request.Description;
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
