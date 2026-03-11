namespace Contracts.Courses.Events;

public class CourseUpdateRejectionEvent
{
  public Guid CourseId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
