namespace Contracts.Classes.Events;

public class ClassDeleteRejectionEvent
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
  public string Reason { get; set; }
  public string UserId { get; init; }
}
