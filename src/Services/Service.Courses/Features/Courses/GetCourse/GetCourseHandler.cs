using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public class GetCourseHandler: IRequestHandler<GetCourseQuery, ErrorOr<Course>>
{
    private readonly ILogger<GetCourseHandler> _logger;
    private readonly ApplicationDbContext _dbContext;

    public GetCourseHandler(ILogger<GetCourseHandler> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Course>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await _dbContext.Courses.FirstOrDefaultAsync(course => course.Id == request.Id, cancellationToken: cancellationToken);
        if (course != null) return course;
        
        _logger.Log(LogLevel.Warning, "Course with id {RequestId} was not found", request.Id);
        return Error.NotFound("courses_service.get_course.not_found", description: $"Course with id {request.Id} was not found");
    }
}