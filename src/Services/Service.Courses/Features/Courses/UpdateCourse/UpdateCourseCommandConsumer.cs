using Contracts.Courses.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommandConsumer : IHandleMessages<Contracts.Courses.Commands.UpdateCourseCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public UpdateCourseCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Courses.Commands.UpdateCourseCommand message)
  {
    var command = new UpdateCourseCommand
    {
      Id = message.CourseId, Name = message.Name, Description = message.Description
    };
    var result = await _mediator.Send(command);
    if (result.IsError)
    {
      await _bus.Publish(new CourseUpdateRejectionEvent
      {
        CourseId = message.CourseId, UserId = message.UserId, Reason = result.FirstError.Description
      });
      return;
    }

    await _bus.Publish(new CourseUpdatedEvent { CourseId = message.CourseId, UserId = message.UserId });
  }
}
