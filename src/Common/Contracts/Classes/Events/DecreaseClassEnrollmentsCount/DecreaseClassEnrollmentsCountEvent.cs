using Contracts.Common;

namespace Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

public class DecreaseClassEnrollmentsCountEvent : Event
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
