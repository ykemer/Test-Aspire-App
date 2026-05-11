using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class IncreaseStudentEnrollmentsEventConsumer : IHandleMessages<IncreaseStudentEnrollmentsCountEvent>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public IncreaseStudentEnrollmentsEventConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(IncreaseStudentEnrollmentsCountEvent message)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(message.StudentId, true));
    if (result.IsError)
    {
      await _bus.Publish(new IncreaseStudentEnrollmentsCountFailedEvent
      {
        StudentId = message.StudentId,
        EventId = message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await _bus.Publish(new IncreaseStudentEnrollmentsCountSuccessEvent
      {
        StudentId = message.StudentId, EventId = message.EventId
      });
    }
  }
}
