namespace Contracts.Classes.Events;

public class ClassCreateRejectionEvent
{
  public string CourseId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
