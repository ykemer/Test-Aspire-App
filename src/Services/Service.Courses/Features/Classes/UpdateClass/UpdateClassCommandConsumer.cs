using Contracts.Classes.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Classes.UpdateClass;

public class UpdateClassCommandConsumer : IHandleMessages<Contracts.Classes.Commands.UpdateClassCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public UpdateClassCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Classes.Commands.UpdateClassCommand message)
  {
    var command = new UpdateClassCommand
    {
      Id = message.ClassId,
      CourseId = message.CourseId,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseStartDate = message.CourseStartDate,
      CourseEndDate = message.CourseEndDate,
      MaxStudents = message.MaxStudents
    };
    var result = await _mediator.Send(command);
    if (result.IsError)
    {
      await _bus.Publish(new ClassUpdateRejectionEvent
      {
        CourseId = message.CourseId,
        UserId = message.UserId,
        Reason = result.FirstError.Description,
        ClassId = message.ClassId
      });
      return;
    }

    await _bus.Publish(new ClassUpdatedEvent
    {
      Id = message.ClassId,
      CourseId = message.CourseId,
      MaxStudents = message.MaxStudents,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseStartDate = message.CourseStartDate,
      CourseEndDate = message.CourseEndDate,
      UserId = message.UserId
    });
  }
}
