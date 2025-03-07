namespace Contracts.Courses.Events;

public record CourseDeletedEvent
{
  public string CourseId { get; set; }
}
