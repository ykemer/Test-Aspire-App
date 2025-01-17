using System.Text.Json;

using Contracts.AsyncMessages;
using Contracts.Enrollments.Events;

using Library.AsyncMessages;

namespace Service.Enrollments.AsyncDataServices;

public class MessageBusClient : AsyncMessenger<MessageBusClient>, IMessageBusClient
{
  public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger) : base(logger,
    configuration)
  {
  }

  public void PublishStudentEnrolledEvent(StudentEnrolledEvent studentEnrolledEvent)
  {
    studentEnrolledEvent.EventType = AsyncEventType.StudentEnrolled;
    var messageObject = JsonSerializer.Serialize(studentEnrolledEvent);
    SendMessage(messageObject);
  }

  public void PublishStudentUnenrolledEvent(StudentUnenrolledEvent studentUnenrolledEvent)
  {
    studentUnenrolledEvent.EventType = AsyncEventType.StudentUnenrolled;
    var messageObject = JsonSerializer.Serialize(studentUnenrolledEvent);
    SendMessage(messageObject);
  }
}
