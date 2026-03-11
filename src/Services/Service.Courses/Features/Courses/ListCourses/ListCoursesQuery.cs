using Contracts.Common;

using Service.Courses.Common.Database.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesRequest : PagedQuery, IRequest<ErrorOr<PagedList<Course>>>
{
  public List<Guid> EnrolledClasses { get; set; } = new();
  public bool ShowAll { get; set; }
}
