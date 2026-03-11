using Contracts.Common;

namespace Contracts.Courses.Requests;

public class ListCoursesRequest : PagedQuery
{
  public required Guid ClassId { get; set; }
}
