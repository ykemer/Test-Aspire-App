using Contracts.Common;

namespace Contracts.Students.Events.DecreaseStudentEnrollmentCount;

public class DecreaseStudentEnrollmentCountSuccessEvent : Event
{
  public string StudentId { get; set; }
}
