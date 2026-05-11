using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.UpdateClass;

public class ClassUpdateRejectionEventConsumer : IHandleMessages<ClassUpdateRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassUpdateRejectionEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassUpdateRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassUpdateRequestRejected, message.Reason);
}
