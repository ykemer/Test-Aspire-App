using Library.AsyncMessages;

namespace Service.Enrollments.AsyncDataServices;

public class MessageBusSubscriber : AsyncMessageHandler<MessageBusSubscriber>
{
  public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor,
    ILogger<MessageBusSubscriber> logger) : base(configuration, eventProcessor, logger)
  {
  }
}
