namespace Contracts.Courses.Events;

public class CourseUpdateRejectionEvent
{
  public string CourseId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
