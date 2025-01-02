using System.Text.Json;
using Contracts.AsyncMessages;
using Contracts.Students.Events;
using Library.AsyncMessages;

namespace Service.Students.AsyncDataServices;

public class MessageBusClient : AsyncMessenger<MessageBusClient>, IMessageBusClient
{
    public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger) : base(logger,
        configuration)
    {
    }
   
    public void PublishStudentDeletedEvent(StudentDeletedEvent studentDeletedEvent)
    {
        studentDeletedEvent.EventType = AsyncEventType.StudentDeleted;
        var messageObject = JsonSerializer.Serialize(studentDeletedEvent);
        SendMessage(messageObject);
    }
}