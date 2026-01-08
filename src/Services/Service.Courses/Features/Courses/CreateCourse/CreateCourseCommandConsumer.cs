using Contracts.Courses.Events;

using MassTransit;

namespace Service.Courses.Features.Courses.CreateCourse;

public class CreateCourseCommandConsumer : IConsumer<Contracts.Courses.Commands.CreateCourseCommand>
{
  private readonly IMediator _mediator;

  public CreateCourseCommandConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<Contracts.Courses.Commands.CreateCourseCommand> context)
  {
    var message = context.Message;
    var command = new CreateCourseCommand(message.Name, message.Description);
    var result = await _mediator.Send(command);
    if (result.IsError)
    {
      await context.Publish(new CourseCreateRejectionEvent
      {
        Reason = result.FirstError.Description, UserId = message.UserId
      });
      return;
    }

    await context.Publish(new CourseCreatedEvent { CourseId = result.Value.Id, UserId = message.UserId });
  }
}
