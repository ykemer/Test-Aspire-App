using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using MassTransit;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class DecreaseStudentEnrollmentsEventConsumer
  : IConsumer<DecreaseStudentEnrollmentCountEvent>
{
  private readonly IMediator _mediator;

  public DecreaseStudentEnrollmentsEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<DecreaseStudentEnrollmentCountEvent> context)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, false));
    if (result.IsError)
    {
      await context.Publish(new DecreaseStudentEnrollmentCountFailedEvent
      {
        StudentId = context.Message.StudentId,
        EventId = context.Message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await context.Publish(new DecreaseStudentEnrollmentCountSuccessEvent
      {
        StudentId = context.Message.StudentId, EventId = context.Message.EventId
      });
    }
  }
}
