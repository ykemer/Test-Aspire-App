using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountFailedEvent : Event
{
  public Guid StudentId { get; set; }
  public string ErrorMessage { get; set; }
}
