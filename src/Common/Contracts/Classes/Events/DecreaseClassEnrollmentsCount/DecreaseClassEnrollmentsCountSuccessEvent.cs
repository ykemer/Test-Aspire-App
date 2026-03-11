using Contracts.Common;

namespace Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountSuccessEvent : Event
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
}
