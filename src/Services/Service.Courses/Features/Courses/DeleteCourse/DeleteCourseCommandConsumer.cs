using MassTransit;

namespace Service.Courses.Features.Courses.DeleteCourse;

public class DeleteCourseCommandConsumer : IConsumer<Contracts.Courses.Commands.DeleteCourseCommand>
{
  private readonly IMediator _mediator;

  public DeleteCourseCommandConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<Contracts.Courses.Commands.DeleteCourseCommand> context)
  {
    var message = context.Message;
    var command = new DeleteCourseCommand(message.CourseId);
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await context.Publish(new Contracts.Courses.Events.CourseDeleteRejectionEvent
      {
        Reason = result.FirstError.Description, CourseId = message.CourseId, UserId = message.UserId
      });
    }
    else
    {
      await context.Publish(
        new Contracts.Courses.Events.CourseDeletedEvent { CourseId = message.CourseId, UserId = message.UserId });
    }
  }
}
