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
    var command = new UpdateNumberOfEnrolledStudentsCommand(
      message.EventId, message.CourseId, message.ClassId, EnrollmentChange.AddStudent);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new IncreaseClassEnrollmentsCountFailedEvent
      {
        CourseId = message.CourseId,
        ClassId = message.ClassId,
        EventId = message.EventId,
        ErrorMessage = result.FirstError.Description
      });
      return;
    }

    await _bus.Publish(new IncreaseClassEnrollmentsCountSuccessEvent
    {
      CourseId = message.CourseId, EventId = message.EventId
    });
  }
}
