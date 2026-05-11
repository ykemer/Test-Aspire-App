using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.DeleteClass;

public class ClassDeletedRejectionEventConsumer : IHandleMessages<ClassDeleteRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassDeletedRejectionEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassDeleteRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassDeleteRequestRejected, message.Reason);
}
