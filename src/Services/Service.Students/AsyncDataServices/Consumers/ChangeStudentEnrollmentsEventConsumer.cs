using Contracts.Students.Events.ChangeStudentEnrollmentsCount;

using MassTransit;

using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices.Consumers;

public class ChangeStudentEnrollmentsEventConsumer : IConsumer<ChangeStudentEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;
  private readonly IPublishEndpoint _publishEndpoint;

  public ChangeStudentEnrollmentsEventConsumer(IMediator mediator, IPublishEndpoint publishEndpoint)
  {
    _mediator = mediator;
    _publishEndpoint = publishEndpoint;
  }

  public async Task Consume(ConsumeContext<ChangeStudentEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, context.Message.IsIncrease));
    if (result.IsError)
    {
      await _publishEndpoint.Publish(new ChangeStudentEnrollmentsCountFailedEvent
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description,
      });
    }
    else
    {
      await _publishEndpoint.Publish(new ChangeStudentEnrollmentsCountSuccessEvent()
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
      });
    }
  }

}
