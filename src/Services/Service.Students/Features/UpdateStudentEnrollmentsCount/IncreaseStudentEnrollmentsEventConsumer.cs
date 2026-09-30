using Contracts.Students.Events.IncreaseStudentEnrollmentsCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

/// <summary>
/// Part of the Platform "enroll" saga: adds one enrollment to the student and reports success or failure.
/// </summary>
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
    var command = new UpdateStudentEnrollmentsCountCommand(
      message.EventId, message.StudentId, EnrollmentChange.AddEnrollment);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new IncreaseStudentEnrollmentsCountFailedEvent
      {
        StudentId = message.StudentId, EventId = message.EventId, ErrorMessage = result.FirstError.Description
      });
      return;
    }

    await _bus.Publish(new IncreaseStudentEnrollmentsCountSuccessEvent
    {
      StudentId = message.StudentId, EventId = message.EventId
    });
  }
}
