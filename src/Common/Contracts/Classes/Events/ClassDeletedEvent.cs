namespace Contracts.Classes.Events;

public record ClassDeletedEvent
{
  public Guid CourseId { get; set; }
  public Guid ClassId { get; set; }
  public string UserId { get; init; }
}
