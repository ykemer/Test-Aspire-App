namespace Contracts.Students.Events;

public record StudentDeletedEvent
{
  public Guid StudentId { get; set; }
}
