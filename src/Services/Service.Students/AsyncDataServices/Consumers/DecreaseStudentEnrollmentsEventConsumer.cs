using Contracts.Students.Events;

using MassTransit;

using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Service.Students.AsyncDataServices.Consumers;

public class DecreaseStudentEnrollmentsEventConsumer: IConsumer<DecreaseStudentEnrollmentsEvent>
{
  private readonly IMediator _mediator;

  public DecreaseStudentEnrollmentsEventConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<DecreaseStudentEnrollmentsEvent> context)
  {
    await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(context.Message.StudentId, false));
  }
}
