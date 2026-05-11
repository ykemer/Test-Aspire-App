using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Classes.DeleteClass;

public class ClassDeletedEventConsumer : IHandleMessages<ClassDeletedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassDeletedEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(ClassDeletedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(ClassHubMessage.ClassDeleted, "Class deleted successfully.");
}
