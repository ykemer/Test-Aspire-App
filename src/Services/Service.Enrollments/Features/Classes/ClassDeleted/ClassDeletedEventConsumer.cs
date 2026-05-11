using Contracts.Classes.Events;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.ClassDeleted;

public class ClassDeletedEventConsumer : IHandleMessages<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassDeletedEvent message) =>
    await _mediator.Send(new DeleteClassByClassIdCommand(message.CourseId, message.ClassId));
}
