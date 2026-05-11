using Contracts.Classes.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommandConsumer : IHandleMessages<Contracts.Classes.Commands.CreateClassCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public CreateClassCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Classes.Commands.CreateClassCommand message)
  {
    var command = new CreateClassCommand
    {
      CourseId = message.CourseId,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseStartDate = message.CourseStartDate,
      CourseEndDate = message.CourseEndDate,
      MaxStudents = message.MaxStudents
    };
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new ClassCreateRejectionEvent
      {
        CourseId = message.CourseId, Reason = result.FirstError.Description, UserId = message.UserId
      });
      return;
    }

    await _bus.Publish(new ClassCreatedEvent
    {
      CourseId = result.Value.CourseId,
      Id = result.Value.Id,
      MaxStudents = result.Value.MaxStudents,
      RegistrationDeadline = result.Value.RegistrationDeadline,
      CourseStartDate = result.Value.CourseStartDate,
      CourseEndDate = result.Value.CourseEndDate,
      UserId = message.UserId
    });
  }
}
