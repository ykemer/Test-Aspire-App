using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.UpdateClass;

public class ClassUpdatedEventConsumer : IHandleMessages<ClassUpdatedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassUpdatedEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassUpdatedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassUpdated, "Class updated successfully.");
}
