using Contracts.Classes.Events;

using Library.Messaging;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.CreateClass;

/// <summary>
/// Copies a new class from the Courses service. If that fails, the message is retried (never silently lost).
/// </summary>
public class ClassCreatedEventConsumer : IHandleMessages<ClassCreatedEvent>
{
  private readonly IMediator _mediator;

  public ClassCreatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassCreatedEvent message)
  {
    var result = await _mediator.Send(message.ToCreateClassCommand());
    result.ThrowIfFailed($"Copying new class {message.Id}");
  }
}
