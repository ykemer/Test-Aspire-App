using Contracts.Classes.Events.IncreaseClassEnrollmentsCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public class IncreaseCourseEnrollmentsCountEventConsumer : IHandleMessages<IncreaseClassEnrollmentsCountEvent>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public IncreaseCourseEnrollmentsCountEventConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(IncreaseClassEnrollmentsCountEvent message)
  {
    var result =
      await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(message.CourseId, message.ClassId, true));
    if (result.IsError)
    {
      await _bus.Publish(new IncreaseClassEnrollmentsCountFailedEvent
      {
        CourseId = message.CourseId,
        ClassId = message.ClassId,
        EventId = message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await _bus.Publish(new IncreaseClassEnrollmentsCountSuccessEvent
      {
        CourseId = message.CourseId, EventId = message.EventId
      });
    }
  }
}
