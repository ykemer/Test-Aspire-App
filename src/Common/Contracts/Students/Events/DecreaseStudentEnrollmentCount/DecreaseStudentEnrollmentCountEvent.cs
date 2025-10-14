using Contracts.Common;

namespace Contracts.Students.Events.DecreaseStudentEnrollmentCount;

public class DecreaseStudentEnrollmentCountEvent: Event
{
  public string StudentId { get; init; }
}
