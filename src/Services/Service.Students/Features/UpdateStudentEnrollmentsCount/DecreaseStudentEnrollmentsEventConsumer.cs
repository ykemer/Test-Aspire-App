using Contracts.Students.Events.DecreaseStudentEnrollmentCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

/// <summary>
/// Part of the Platform "unenroll" saga: removes one enrollment from the student and reports success or failure.
/// </summary>
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
    var command = new UpdateStudentEnrollmentsCountCommand(
      message.EventId, message.StudentId, EnrollmentChange.RemoveEnrollment);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new DecreaseStudentEnrollmentCountFailedEvent
      {
        StudentId = message.StudentId, EventId = message.EventId, ErrorMessage = result.FirstError.Description
      });
      return;
    }

    await _bus.Publish(new DecreaseStudentEnrollmentCountSuccessEvent
    {
      StudentId = message.StudentId, EventId = message.EventId
    });
  }
}
