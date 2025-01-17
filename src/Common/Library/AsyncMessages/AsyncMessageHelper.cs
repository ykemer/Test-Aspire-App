using System.Text.Json;

using Contracts.AsyncMessages;

namespace Library.AsyncMessages;

public static class AsyncMessageHelper
{
  public static AsyncEventType DetermineEvent(string notification)
  {
    var asyncEvent = JsonSerializer.Deserialize<AsyncMessage>(notification);
    return asyncEvent.EventType;
  }
}
