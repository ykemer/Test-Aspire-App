using Contracts.Common;

namespace Contracts.Courses.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountSuccessEvent: Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
