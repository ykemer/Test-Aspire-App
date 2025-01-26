namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class
  UpdateStudentEnrollmentsCountHandler : IRequestHandler<UpdateStudentEnrollmentsCountCommand, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<UpdateStudentEnrollmentsCountHandler> _logger;

  public UpdateStudentEnrollmentsCountHandler(ILogger<UpdateStudentEnrollmentsCountHandler> logger,
    ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<ErrorOr<Updated>> Handle(UpdateStudentEnrollmentsCountCommand request,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Updating student {StudentId} enrollments count", request.StudentId);
    var student = await _dbContext.Students.FindAsync(request.StudentId, cancellationToken);
    if (student == null)
    {
      _logger.LogError("Student {StudentId} not found", request.StudentId);
      return Error.Conflict("student_service.update_student_enrollments_count.student_not_found",
        $"Student {request.StudentId} not found");
    }

    student.EnrolledCourses += request.Increase ? 1 : -1;
    if(student.EnrolledCourses < 0)
    {
      _logger.LogError("Student {StudentId} enrollments count cannot be negative", request.StudentId);
      return Error.Conflict("student_service.update_student_enrollments_count.invalid_enrollments_count",
        "Student enrollments count cannot be negative");
    }

    await _dbContext.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
