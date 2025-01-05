namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class UpdateStudentEnrollmentsCountHandler: IRequestHandler<UpdateStudentEnrollmentsCountCommand, ErrorOr<Updated>>
{
    private readonly ILogger<UpdateStudentEnrollmentsCountHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public UpdateStudentEnrollmentsCountHandler(ILogger<UpdateStudentEnrollmentsCountHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Updated>> Handle(UpdateStudentEnrollmentsCountCommand request, CancellationToken cancellationToken)
    {
        var student = _dbContext.Students.Find(request.StudentId);
        if (student == null)
        {
            _logger.LogError("Student {StudentId} not found", request.StudentId);
            return Error.Conflict("student_service.update_student_enrollments_count.student_not_found",
                $"Student {request.StudentId} not found");
        }
        
        student.EnrolledCourses += request.Increase ? 1 : -1;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}