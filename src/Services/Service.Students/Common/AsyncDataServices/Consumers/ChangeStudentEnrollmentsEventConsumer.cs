using Contracts.Students.Events.ChangeStudentEnrollmentsCount;

using MassTransit;

using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices.Consumers;

public class ChangeStudentEnrollmentsEventConsumer : IConsumer<ChangeStudentEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;

  public ChangeStudentEnrollmentsEventConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<ChangeStudentEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, context.Message.IsIncrease));
    if (result.IsError)
    {
      await context.Publish(new ChangeStudentEnrollmentsCountFailedEvent
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await context.Publish(new ChangeStudentEnrollmentsCountSuccessEvent()
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
      });
    }
  }

}
