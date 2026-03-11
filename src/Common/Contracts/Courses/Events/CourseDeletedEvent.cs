namespace Contracts.Courses.Events;

public record CourseDeletedEvent
{
  public Guid CourseId { get; set; }
  public string UserId { get; init; }
}
