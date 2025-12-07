namespace Contracts.Classes.Events;

public class ClassUpdateRejectionEvent
{
  public string CourseId { get; init; }
  public string ClassId { get; init; }
  public string UserId { get; init; }
  public string Reason { get; init; }
}
