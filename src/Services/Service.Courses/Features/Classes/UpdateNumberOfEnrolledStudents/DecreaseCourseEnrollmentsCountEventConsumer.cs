using Contracts.Classes.Events.DecreaseClassEnrollmentsCount;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public class DecreaseCourseEnrollmentsCountEventConsumer : IHandleMessages<DecreaseClassEnrollmentsCountEvent>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public DecreaseCourseEnrollmentsCountEventConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(DecreaseClassEnrollmentsCountEvent message)
  {
    var result =
      await _mediator.Send(new UpdateNumberOfEnrolledStudentsCommand(message.CourseId, message.ClassId, false));
    if (result.IsError)
    {
      await _bus.Publish(new DecreaseClassEnrollmentsCountFailedEvent
      {
        CourseId = message.CourseId,
        ClassId = message.ClassId,
        EventId = message.EventId,
        ErrorMessage = result.Errors.FirstOrDefault().Description
      });
    }
    else
    {
      await _bus.Publish(new DecreaseClassEnrollmentsCountSuccessEvent
      {
        CourseId = message.CourseId,
        ClassId = message.ClassId,
        EventId = message.EventId
      });
    }
  }
}
