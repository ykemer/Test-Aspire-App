using Contracts.Users.Events;

namespace Platform.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishUserRegisteredMessage(UserCreatedEvent userRegisteredMessage);
}