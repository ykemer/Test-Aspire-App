using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class StudentEnrollmentCountChangedEvent : Event
{
  public Guid CourseId { get; init; }
  public Guid ClassId { get; init; }
  public Guid StudentId { get; init; }
  public bool IsIncrease { get; init; }
}
