using Contracts.Common;

namespace Contracts.Students.Events.DecreaseStudentEnrollmentCount;

public class DecreaseStudentEnrollmentCountFailedEvent: Event
{
  public string StudentId { get; set; }
  public string ErrorMessage { get; set; }
}
