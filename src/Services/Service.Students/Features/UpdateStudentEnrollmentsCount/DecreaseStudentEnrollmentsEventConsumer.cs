using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public class DecreaseStudentEnrollmentsEventConsumer : IHandleMessages<DecreaseStudentEnrollmentCountEvent>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public DecreaseStudentEnrollmentsEventConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(DecreaseStudentEnrollmentCountEvent message)
  {
    var result = await _mediator.Send(new UpdateStudentEnrollmentsCountCommand(message.StudentId, false));
    if (result.IsError)
    {
      await _bus.Publish(new DecreaseStudentEnrollmentCountFailedEvent
      {
        StudentId = message.StudentId,
        EventId = message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await _bus.Publish(new DecreaseStudentEnrollmentCountSuccessEvent
      {
        StudentId = message.StudentId, EventId = message.EventId
      });
    }
  }
}
