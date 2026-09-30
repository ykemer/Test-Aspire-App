using Contracts.Classes.Events;

using Library.Messaging;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.ClassDeleted;

/// <summary>
/// Removes a deleted class. If that fails, the message is retried (never silently lost).
/// </summary>
public class ClassDeletedEventConsumer : IHandleMessages<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassDeletedEvent message)
  {
    var result = await _mediator.Send(new DeleteClassByClassIdCommand(message.CourseId, message.ClassId));
    result.ThrowIfFailed($"Deleting class {message.ClassId}");
  }
}
