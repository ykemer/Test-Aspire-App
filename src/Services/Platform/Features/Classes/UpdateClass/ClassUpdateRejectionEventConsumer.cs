using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.UpdateClass;

public class ClassUpdateRejectionEventConsumer: IConsumer<ClassUpdateRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassUpdateRejectionEventConsumer(IHubContext<ClassesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<ClassUpdateRejectionEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassUpdateRequestRejected,context.Message.Reason);
  }
}
