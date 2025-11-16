using Contracts.Common;

namespace Contracts.Courses.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountSuccessEvent : Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
