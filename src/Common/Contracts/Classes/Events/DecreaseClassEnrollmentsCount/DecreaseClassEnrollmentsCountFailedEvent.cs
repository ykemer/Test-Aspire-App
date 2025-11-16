using Contracts.Common;

namespace Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountFailedEvent : Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public string ErrorMessage { get; set; }
}
