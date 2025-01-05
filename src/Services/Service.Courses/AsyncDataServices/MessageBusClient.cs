using System.Text;
using System.Text.Json;
using Contracts.AsyncMessages;
using Contracts.Courses.Events;
using Library.AsyncMessages;
using RabbitMQ.Client;
using Service.Courses.Entities;

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