using Contracts.Common;

namespace Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountEvent : Event
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
}
