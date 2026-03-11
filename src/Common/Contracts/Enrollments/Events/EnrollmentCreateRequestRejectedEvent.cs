using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class EnrollmentCreateRequestRejectedEvent : Event
{
  public Guid CourseId { get; init; }
  public Guid ClassId { get; init; }
  public Guid StudentId { get; init; }
  public string Reason { get; init; }
}
