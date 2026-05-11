using Contracts.Classes.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Classes.DeleteClass;

public class DeleteClassCommandConsumer : IHandleMessages<Contracts.Classes.Commands.DeleteClassCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public DeleteClassCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Classes.Commands.DeleteClassCommand message)
  {
    var command = new DeleteClassCommand(message.ClassId, message.CourseId);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new ClassDeleteRejectionEvent
      {
        Reason = result.FirstError.Description,
        CourseId = message.CourseId,
        UserId = message.UserId,
        ClassId = message.ClassId
      });
      return;
    }

    await _bus.Publish(
      new ClassDeletedEvent { CourseId = message.CourseId, UserId = message.UserId, ClassId = message.ClassId });
  }
}
