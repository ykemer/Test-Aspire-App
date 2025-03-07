namespace Contracts.Common;

public class Event
{
  public Guid EventId { get; set; } = Guid.NewGuid();
}
