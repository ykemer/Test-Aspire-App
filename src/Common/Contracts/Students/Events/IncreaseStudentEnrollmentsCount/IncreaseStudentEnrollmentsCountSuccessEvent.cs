using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountSuccessEvent : Event
{
  public Guid StudentId { get; set; }
}
