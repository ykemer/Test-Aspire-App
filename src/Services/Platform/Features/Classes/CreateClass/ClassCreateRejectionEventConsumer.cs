using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.CreateClass;

public class ClassCreateRejectionEventConsumer : IHandleMessages<ClassCreateRejectionEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassCreateRejectionEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassCreateRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassCreateRequestRejected, message.Reason);
}
