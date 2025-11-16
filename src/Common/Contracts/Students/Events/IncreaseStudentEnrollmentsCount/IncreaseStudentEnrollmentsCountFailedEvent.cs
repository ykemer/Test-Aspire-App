using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountFailedEvent : Event
{
  public string StudentId { get; set; }
  public string ErrorMessage { get; set; }
}
