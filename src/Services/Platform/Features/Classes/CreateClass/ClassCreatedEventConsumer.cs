using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.CreateClass;

public class ClassCreatedEventConsumer : IHandleMessages<ClassCreatedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassCreatedEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassCreatedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassCreated, "Class created successfully.");
}
