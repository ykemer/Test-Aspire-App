using Contracts.Common;

namespace Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountFailedEvent : Event
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public string ErrorMessage { get; set; }
}
