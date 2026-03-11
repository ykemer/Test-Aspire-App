namespace Contracts.Classes.Events;

public class ClassCreateRejectionEvent
{
  public Guid CourseId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
