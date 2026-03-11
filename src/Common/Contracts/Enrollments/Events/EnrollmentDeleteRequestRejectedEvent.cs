using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class EnrollmentDeleteRequestRejectedEvent : Event
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
  public required string Reason { get; init; }
}
