using Contracts.Common;

namespace Contracts.Courses.Requests;

public class ListCoursesRequest : PagedQuery
{
  public required string ClassId { get; set; }
}
