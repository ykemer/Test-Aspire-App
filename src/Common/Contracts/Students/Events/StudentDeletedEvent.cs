namespace Contracts.Students.Events;

public record StudentDeletedEvent
{
  public string StudentId { get; set; }
}
