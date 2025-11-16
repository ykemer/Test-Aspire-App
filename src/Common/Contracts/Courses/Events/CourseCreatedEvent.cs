namespace Contracts.Courses.Events;

public class CourseCreatedEvent
{
  public string CourseId { get; init; }
  public string UserId { get; init; }
}
