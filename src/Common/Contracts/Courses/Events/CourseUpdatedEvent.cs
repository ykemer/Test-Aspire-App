namespace Contracts.Courses.Events;

public class CourseUpdatedEvent
{
  public string CourseId { get; init; }
  public string UserId { get; init; }
}
