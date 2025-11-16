using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using MassTransit;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsEventConsumer
  : IConsumer<IncreaseStudentEnrollmentsCountEvent>
{
  private readonly IMediator _mediator;

  public IncreaseStudentEnrollmentsEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<IncreaseStudentEnrollmentsCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, true));
    if (result.IsError)
    {
      await context.Publish(new IncreaseStudentEnrollmentsCountFailedEvent
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await context.Publish(new IncreaseStudentEnrollmentsCountSuccessEvent
      {
        StudentId = context.Message.StudentId, EventId = context.Message.EventId
      });
    }
  }
}
