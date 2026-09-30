namespace Service.Students.Common.Database.Entities;

/// <summary>
/// Inbox pattern: remembers every message this service already handled.
/// When the message broker delivers the same message again, the copy is ignored instead of applied twice.
/// (Rebus has no built-in consumer inbox, so this table plays that role.)
/// </summary>
public class InboxMessage
{
  public required Guid MessageId { get; init; }
  public required string MessageType { get; init; }
  public required DateTime ProcessedAt { get; init; }
}
