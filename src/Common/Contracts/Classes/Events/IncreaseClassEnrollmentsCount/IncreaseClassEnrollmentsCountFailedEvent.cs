using Contracts.Common;

namespace Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountFailedEvent : Event
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public string ErrorMessage { get; set; }
}
