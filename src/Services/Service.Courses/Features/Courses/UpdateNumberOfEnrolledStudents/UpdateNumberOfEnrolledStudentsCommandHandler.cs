namespace Service.Courses.Features.Courses.UpdateNumberOfEnrolledStudents;

public class
  UpdateNumberOfEnrolledStudentsCommandHandler : IRequestHandler<UpdateNumberOfEnrolledStudentsCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> _logger;

  public UpdateNumberOfEnrolledStudentsCommandHandler(ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> logger,
    ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Updated>> Handle(UpdateNumberOfEnrolledStudentsCommand request,
    CancellationToken cancellationToken)
  {
    var existingCourse = await _dbContext.Courses
      .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
    if (existingCourse == null)
    {
      _logger.LogWarning("Course {CourseId} not found", request.CourseId);
      return Error.NotFound("course_service.increase_number_of_enrolled_students.course.not_found",
        $"Course {request.CourseId} not found");
    }
    existingCourse.EnrollmentsCount += request.IsIncrease ? 1 : -1;
    if(existingCourse.EnrollmentsCount < 0)
    {
      _logger.LogError("Course {CourseId} enrollments count cannot be negative", request.CourseId);
      return Error.Conflict("course_service.update_course_enrollments_count.invalid_enrollments_count",
        "Course enrollments count cannot be negative");
    }

    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
