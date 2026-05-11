using Contracts.Courses.Events;

using Rebus.Bus;
using Rebus.Handlers;

namespace Service.Courses.Features.Courses.DeleteCourse;

public class DeleteCourseCommandConsumer : IHandleMessages<Contracts.Courses.Commands.DeleteCourseCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public DeleteCourseCommandConsumer(IMediator mediator, IBus bus)
  {
    _mediator = mediator;
    _bus = bus;
  }

  public async Task Handle(Contracts.Courses.Commands.DeleteCourseCommand message)
  {
    var command = new DeleteCourseCommand(message.CourseId);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await _bus.Publish(new CourseDeleteRejectionEvent
      {
        Reason = result.FirstError.Description, CourseId = message.CourseId, UserId = message.UserId
      });
      return;
    }

    await _bus.Publish(new CourseDeletedEvent { CourseId = message.CourseId, UserId = message.UserId });
  }
}
