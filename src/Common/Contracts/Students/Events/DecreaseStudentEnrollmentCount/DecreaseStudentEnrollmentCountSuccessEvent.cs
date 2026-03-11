using Contracts.Common;

namespace Contracts.Students.Events.DecreaseStudentEnrollmentCount;

public class DecreaseStudentEnrollmentCountSuccessEvent : Event
{
  public Guid StudentId { get; set; }
}
