using Contracts.Courses.Events;

using MassTransit;

namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommandConsumer : IConsumer<Contracts.Courses.Commands.UpdateCourseCommand>
{
  private readonly IMediator _mediator;

  public UpdateCourseCommandConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<Contracts.Courses.Commands.UpdateCourseCommand> context)
  {
    var message = context.Message;
    var command = new UpdateCourseCommand
    {
      Id = message.CourseId, Name = message.Name, Description = message.Description
    };
    var result = await _mediator.Send(command);
    if (result.IsError)
    {
      await context.Publish(new CourseUpdateRejectionEvent
      {
        CourseId = message.CourseId, UserId = message.UserId, Reason = result.FirstError.Description
      });
      return;
    }

    await context.Publish(new CourseUpdatedEvent { CourseId = message.CourseId, UserId = message.UserId });
  }
}
