using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class StudentEnrollmentCountChangedEvent: Event
{
  public string CourseId { get; set; }
  public string StudentId { get; set; }
  public bool IsIncrease { get; set; }
}
