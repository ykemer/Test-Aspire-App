namespace Contracts.Courses.Events;

public class CourseCreateRejectionEvent
{
  public string UserId { get; init; }
  public string Reason { get; init; }
}
