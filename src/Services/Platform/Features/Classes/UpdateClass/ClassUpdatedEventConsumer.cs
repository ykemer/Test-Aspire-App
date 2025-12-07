using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.UpdateClass;

public class ClassUpdatedEventConsumer: IConsumer<ClassUpdatedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassUpdatedEventConsumer(IHubContext<ClassesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<ClassUpdatedEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassUpdated,
        "Class updated successfully.");
  }
}
