using System.Text.Json;

using Contracts.AsyncMessages;
using Contracts.Courses.Events;

using Library.AsyncMessages;

namespace Service.Courses.AsyncDataServices;

public class MessageBusClient : AsyncMessenger<MessageBusClient>, IMessageBusClient
{
  public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger) : base(logger,
    configuration)
  {
  }

  public void PublishCourseDeletedMessage(CourseDeletedEvent courseDeletedEvent)
  {
    courseDeletedEvent.EventType = AsyncEventType.CourseDeleted;
    var messageObject = JsonSerializer.Serialize(courseDeletedEvent);
    SendMessage(messageObject);
  }
}
