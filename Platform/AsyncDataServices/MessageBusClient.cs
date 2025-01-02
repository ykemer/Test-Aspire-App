using System.Text.Json;
using Contracts.AsyncMessages;
using Contracts.Users.Events;
using Library.AsyncMessages;

namespace Platform.AsyncDataServices;

public class MessageBusClient : AsyncMessenger<MessageBusClient>, IMessageBusClient
{
    public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger) : base(logger,
        configuration)
    {
    }

    public void PublishUserRegisteredMessage(UserCreatedEvent userRegisteredMessage)
    {
        userRegisteredMessage.EventType = AsyncEventType.UseCreated;
        var messageObject = JsonSerializer.Serialize(userRegisteredMessage);
        SendMessage(messageObject);
    }
}