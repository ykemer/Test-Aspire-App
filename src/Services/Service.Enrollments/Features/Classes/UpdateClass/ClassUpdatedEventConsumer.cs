using Contracts.Classes.Events;

using Library.Messaging;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.UpdateClass;

/// <summary>
/// Copies class changes from the Courses service. If that fails, the message is retried (never silently lost).
/// </summary>
public class ClassUpdatedEventConsumer : IHandleMessages<ClassUpdatedEvent>
{
  private readonly IMediator _mediator;

  public ClassUpdatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassUpdatedEvent message)
  {
    var result = await _mediator.Send(message.ToUpdateClassCommand());
    result.ThrowIfFailed($"Copying changes of class {message.Id}");
  }
}
