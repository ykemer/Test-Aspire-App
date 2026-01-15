using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.DeleteClass;

public class ClassDeletedRejectionEventConsumer : IConsumer<ClassDeleteRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassDeletedRejectionEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Consume(ConsumeContext<ClassDeleteRejectionEvent> context) =>
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassDeleteRequestRejected,
        context.Message.Reason);
}
