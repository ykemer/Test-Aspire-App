using Contracts.Common;

namespace Contracts.Courses.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountFailedEvent: Event
{

  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public string ErrorMessage { get; set; }
}
