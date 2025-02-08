namespace Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

public class
  IncreaseNumberOfEnrolledStudentsCommandHandler : IRequestHandler<IncreaseNumberOfEnrolledStudentsCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<IncreaseNumberOfEnrolledStudentsCommandHandler> _logger;

  public IncreaseNumberOfEnrolledStudentsCommandHandler(ILogger<IncreaseNumberOfEnrolledStudentsCommandHandler> logger,
    ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<ErrorOr<Updated>> Handle(IncreaseNumberOfEnrolledStudentsCommand request,
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

    existingCourse.EnrollmentsCount++;
    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
