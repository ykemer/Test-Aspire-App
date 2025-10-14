using Contracts.Common;

namespace Contracts.Courses.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountEvent: Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
