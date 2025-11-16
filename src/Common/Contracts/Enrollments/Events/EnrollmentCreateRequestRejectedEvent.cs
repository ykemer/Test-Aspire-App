using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class EnrollmentCreateRequestRejectedEvent : Event
{
  public string CourseId { get; init; }
  public string ClassId { get; init; }
  public string StudentId { get; init; }
  public string Reason { get; init; }
}
