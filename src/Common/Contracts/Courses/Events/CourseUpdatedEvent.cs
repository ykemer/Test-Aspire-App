namespace Contracts.Courses.Events;

public class CourseUpdatedEvent
{
  public Guid CourseId { get; init; }
  public string UserId { get; init; }
}
