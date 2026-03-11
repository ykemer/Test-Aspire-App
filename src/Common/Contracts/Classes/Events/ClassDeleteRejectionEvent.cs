namespace Contracts.Classes.Events;

public class ClassDeleteRejectionEvent
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public string Reason { get; set; }
  public string UserId { get; init; }
}
