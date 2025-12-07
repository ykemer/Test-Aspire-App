using Contracts.Classes.Events;

using MassTransit;

namespace Service.Courses.Features.Classes.DeleteClass;

public class DeleteClassCommandConsumer : IConsumer<Contracts.Classes.Commands.DeleteClassCommand>
{
  private readonly IMediator _mediator;

  public DeleteClassCommandConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<Contracts.Classes.Commands.DeleteClassCommand> context)
  {
    var message = context.Message;
    var command = new DeleteClassCommand(
      message.ClassId,
      message.CourseId
     );
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await context.Publish(new ClassDeleteRejectionEvent()
      {
        Reason = result.FirstError.Description,
        CourseId = message.CourseId,
        UserId = message.UserId,
        ClassId = message.ClassId
      });
      return;
    }

    await context.Publish(
      new ClassDeletedEvent
      {
        CourseId = message.CourseId,
        UserId = message.UserId,
        ClassId = message.ClassId
      });
  }
}
