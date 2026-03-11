namespace Contracts.Courses.Events;

public class CourseDeleteRejectionEvent
{
  public Guid CourseId { get; set; }
  public string Reason { get; set; }
  public string UserId { get; init; }
}
