using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountEvent : Event
{
  public Guid StudentId { get; init; }
}
