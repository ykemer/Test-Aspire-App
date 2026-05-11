using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class CreateEnrollmentCommandConsumer : IHandleMessages<CreateEnrollmentCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public CreateEnrollmentCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(CreateEnrollmentCommand message)
  {
    var result = await _mediator.Send(message.MapToEnrollStudentToClassCommand());

    if (result.IsError)
    {
      await _bus.Publish(new EnrollmentCreateRequestRejectedEvent
      {
        ClassId = message.ClassId,
        CourseId = message.CourseId,
        StudentId = message.StudentId,
        Reason = result.FirstError.Description
      });
    }
    else
    {
      await _bus.Publish(new EnrollmentCreatedEvent
      {
        ClassId = message.ClassId, CourseId = message.CourseId, StudentId = message.StudentId
      });
    }
  }
}
