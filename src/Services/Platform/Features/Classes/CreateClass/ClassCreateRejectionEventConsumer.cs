using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.CreateClass;

public class ClassCreateRejectionEventConsumer : IConsumer<ClassCreateRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassCreateRejectionEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Consume(ConsumeContext<ClassCreateRejectionEvent> context) =>
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassCreateRequestRejected,
        context.Message.Reason);
}
