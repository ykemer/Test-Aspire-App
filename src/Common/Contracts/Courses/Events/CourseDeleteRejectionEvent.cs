namespace Contracts.Courses.Events;

public class CourseDeleteRejectionEvent
{
  public string CourseId { get; set; }
  public string Reason { get; set; }
  public string UserId { get; init; }
}
