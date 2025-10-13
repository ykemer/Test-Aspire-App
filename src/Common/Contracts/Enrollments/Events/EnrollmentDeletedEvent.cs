using Contracts.Common;

namespace Contracts.Enrollments.Events;

public class EnrollmentDeletedEvent: Event
{
  public required string CourseId { get; init; }
  public required string ClassId { get; init; }
  public required string StudentId { get; init; }
}
