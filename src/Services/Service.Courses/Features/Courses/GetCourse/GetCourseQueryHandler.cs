using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;

namespace Service.Courses.Features.Courses.GetCourse;

/// <summary>
/// Returns one course. Unless "show all" is set, the course is returned only if it has a visible class
/// (see <see cref="ClassVisibility"/>).
/// </summary>
public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, ErrorOr<Course>>
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<GetCourseQueryHandler> _logger;
  private readonly TimeProvider _timeProvider;

  public GetCourseQueryHandler(ILogger<GetCourseQueryHandler> logger, ApplicationDbContext dbContext,
    TimeProvider timeProvider)
  {
    _logger = logger;
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<Course>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
  {
    var courses = _dbContext.Courses
      .AsNoTracking()
      .Where(course => course.Id == request.Id);

    if (!request.ShowAll)
    {
      var now = _timeProvider.GetUtcNow().UtcDateTime;
      courses = courses.OnlyCoursesWithVisibleClasses(request.EnrolledClasses, now);
    }

    var foundCourse = await courses.FirstOrDefaultAsync(cancellationToken);
    if (foundCourse is null)
    {
      _logger.LogWarning("Course {CourseId} was not found", request.Id);
      return CourseErrors.NotFound(request.Id);
    }

    return foundCourse;
  }
}
