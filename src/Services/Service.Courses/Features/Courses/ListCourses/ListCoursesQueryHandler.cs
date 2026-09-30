using Contracts.Common;

using Library.Database;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Configurations;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;

namespace Service.Courses.Features.Courses.ListCourses;

/// <summary>
/// Returns one page of courses.
/// Without search text: newest first. With search text: best match first (PostgreSQL full-text search).
/// Unless "show all" is set, only courses with a visible class are returned (see <see cref="ClassVisibility"/>).
/// </summary>
public class ListCoursesQueryHandler : IRequestHandler<ListCoursesQuery, ErrorOr<PagedList<Course>>>
{
  private const string Language = CourseConfiguration.SearchLanguage;

  private readonly ApplicationDbContext _dbContext;
  private readonly TimeProvider _timeProvider;

  public ListCoursesQueryHandler(ApplicationDbContext dbContext, TimeProvider timeProvider)
  {
    _dbContext = dbContext;
    _timeProvider = timeProvider;
  }

  public async ValueTask<ErrorOr<PagedList<Course>>> Handle(ListCoursesQuery request,
    CancellationToken cancellationToken)
  {
    var courses = _dbContext.Courses.AsNoTracking();

    if (!request.ShowAll)
    {
      var now = _timeProvider.GetUtcNow().UtcDateTime;
      courses = courses.OnlyCoursesWithVisibleClasses(request.EnrolledClasses, now);
    }

    var orderedCourses = string.IsNullOrWhiteSpace(request.Query)
      ? courses.OrderByDescending(course => course.CreatedAt)
      : SearchByText(courses, request.Query);

    return await orderedCourses.ToPagedListAsync(request.PageNumber, request.PageSize, cancellationToken);
  }

  // Name + " " + Description must stay written exactly like this: it matches the GIN index
  // defined in CourseConfiguration, which is what makes the search fast.
  private static IOrderedQueryable<Course> SearchByText(IQueryable<Course> courses, string searchText) =>
    courses
      .Where(course => EF.Functions.ToTsVector(Language, course.Name + " " + course.Description)
        .Matches(EF.Functions.PhraseToTsQuery(Language, searchText)))
      .OrderByDescending(course => EF.Functions.ToTsVector(Language, course.Name + " " + course.Description)
        .Rank(EF.Functions.PhraseToTsQuery(Language, searchText)))
      .ThenByDescending(course => course.CreatedAt);
}
