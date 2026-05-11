using Contracts.Courses.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Courses.CreateCourse;

public class CreateCourseCommandConsumer : IHandleMessages<Contracts.Courses.Commands.CreateCourseCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public CreateCourseCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Courses.Commands.CreateCourseCommand message)
  {
    var command = new CreateCourseCommand(message.Name, message.Description);
    var result = await _mediator.Send(command);
    if (result.IsError)
    {
      await _bus.Publish(new CourseCreateRejectionEvent
      {
        Reason = result.FirstError.Description, UserId = message.UserId
      });
      return;
    }

    await _bus.Publish(new CourseCreatedEvent { CourseId = result.Value.Id, UserId = message.UserId });
  }
}
