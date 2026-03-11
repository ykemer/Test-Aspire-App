namespace Contracts.Classes.Events;

public class ClassUpdateRejectionEvent
{
  public Guid CourseId { get; init; }
  public Guid ClassId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
