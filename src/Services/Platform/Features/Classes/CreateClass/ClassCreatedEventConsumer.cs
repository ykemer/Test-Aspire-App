using Contracts.Classes.Events;
using Contracts.Classes.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes.CreateClass;

public class ClassCreatedEventConsumer : IConsumer<ClassCreatedEvent>
{
  private readonly IHubContext<ClassesHub> _hubContext;

  public ClassCreatedEventConsumer(IHubContext<ClassesHub> hubContext) => _hubContext = hubContext;

  public async Task Consume(ConsumeContext<ClassCreatedEvent> context) =>
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(ClassHubMessage.ClassCreated,
        "Class created successfully.");
}
