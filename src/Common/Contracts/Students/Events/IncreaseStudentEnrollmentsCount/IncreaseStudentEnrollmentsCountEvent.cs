using Contracts.Common;

namespace Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsCountEvent : Event
{
  public string StudentId { get; init; }
}
