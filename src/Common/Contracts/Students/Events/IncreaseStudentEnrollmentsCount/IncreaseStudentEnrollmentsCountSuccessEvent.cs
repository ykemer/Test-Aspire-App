using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountSuccessEvent : Event
{
  public string StudentId { get; set; }
}
