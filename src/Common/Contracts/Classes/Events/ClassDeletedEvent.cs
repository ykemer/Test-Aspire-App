namespace Contracts.Classes.Events;

public record ClassDeletedEvent
{
  public string CourseId { get; set; }
  public string ClassId { get; set; }
}
