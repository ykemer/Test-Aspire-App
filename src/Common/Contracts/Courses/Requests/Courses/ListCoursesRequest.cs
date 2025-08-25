using Contracts.Common;

namespace Contracts.Courses.Requests.Courses;

public class ListCoursesRequest : PagedQuery
{
  public required string ClassId { get; set; }
}
