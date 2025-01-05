namespace Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;

public class
    DecreaseNumberOfEnrolledStudentsHandler : IRequestHandler<DecreaseNumberOfEnrolledStudentsCommand, ErrorOr<Updated>>
{
    private readonly ILogger<DecreaseNumberOfEnrolledStudentsHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public DecreaseNumberOfEnrolledStudentsHandler(ILogger<DecreaseNumberOfEnrolledStudentsHandler> logger,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<Updated>> Handle(DecreaseNumberOfEnrolledStudentsCommand request,
        CancellationToken cancellationToken)
    {
        var existingCourse = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken: cancellationToken);
        
        if (existingCourse == null)
        {
            _logger.LogWarning("Course {CourseId} not found", request.CourseId);
            return Error.NotFound("course_service.decrease_number_of_enrolled_students.course.not_found",
                $"Course {request.CourseId} not found");
        }

        if (existingCourse.EnrollmentsCount - 1 < 0)
        {
            _logger.LogWarning("Course {CourseId} has no enrolled students", request.CourseId);
            return Error.Conflict("course_service.decrease_number_of_enrolled_students.course.no_enrolled_students",
                $"Course {request.CourseId} has no enrolled students");
        }

        existingCourse.EnrollmentsCount--;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}