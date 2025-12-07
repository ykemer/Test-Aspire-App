using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.DeleteClass;

public class ClassDeletedEventConsumer: IConsumer<ClassDeletedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassDeletedEventConsumer(IHubContext<ClassesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<ClassDeletedEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassDeleted,
        "Class deleted successfully.");
  }
}
