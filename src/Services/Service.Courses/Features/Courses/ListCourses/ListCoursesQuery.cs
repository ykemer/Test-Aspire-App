using Contracts.Common;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesQuery : PagedQuery, IRequest<ErrorOr<PagedList<Course>>>
{
  public IReadOnlyCollection<Guid> EnrolledClasses { get; init; } = [];
  public bool ShowAll { get; init; }
}
