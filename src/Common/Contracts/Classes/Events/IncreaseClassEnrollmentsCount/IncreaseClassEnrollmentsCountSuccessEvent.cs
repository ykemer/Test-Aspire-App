using Contracts.Common;

namespace Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

public class IncreaseClassEnrollmentsCountSuccessEvent : Event
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
}
