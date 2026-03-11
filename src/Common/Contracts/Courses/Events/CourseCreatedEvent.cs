namespace Contracts.Courses.Events;

public class CourseCreatedEvent
{
  public Guid CourseId { get; init; }
  public string UserId { get; init; }
}
