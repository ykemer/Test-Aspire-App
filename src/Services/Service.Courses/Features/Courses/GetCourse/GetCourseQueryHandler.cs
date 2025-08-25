using Service.Courses.Database.Entities;

namespace Service.Courses.Features.Courses.GetCourse;

public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, ErrorOr<Course>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetCourseQueryHandler> _logger;

  public GetCourseQueryHandler(ILogger<GetCourseQueryHandler> logger, ApplicationDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async ValueTask<ErrorOr<Course>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
  {
    var course = await _dbContext.Courses
      .Include(x => x.CourseClasses)
      .AsSplitQuery()
      .AsNoTracking()
      .Where(x => x.Id == request.Id &&
                  (request.ShowAll || x.CourseClasses.Any(cs =>
                    request.EnrolledClasses.Contains(cs.Id) ||
                    (cs.RegistrationDeadline >= DateTime.UtcNow && cs.TotalStudents < cs.MaxStudents)
                  ))
      )
      .FirstOrDefaultAsync(cancellationToken);


    if (course != null)
    {
      return course;
    }

    _logger.Log(LogLevel.Warning, "Course with id {RequestId} was not found", request.Id);
    return Error.NotFound("courses_service.get_course.not_found", $"Course with id {request.Id} was not found");
  }
}
