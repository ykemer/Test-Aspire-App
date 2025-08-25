using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class StudentEnrollmentCountChangedEvent: Event
{
  public string CourseId { get; init; }
  public string ClassId { get; init; }
  public string StudentId { get; init; }
  public bool IsIncrease { get; init; }
}
