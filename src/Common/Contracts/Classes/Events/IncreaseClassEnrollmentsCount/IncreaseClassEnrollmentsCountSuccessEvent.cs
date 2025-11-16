using Contracts.Common;

namespace Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountSuccessEvent : Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
