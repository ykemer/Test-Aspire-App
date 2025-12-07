using Contracts.Classes.Events;

using MassTransit;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommandConsumer : IConsumer<Contracts.Classes.Commands.CreateClassCommand>
{
  private readonly IMediator _mediator;

  public CreateClassCommandConsumer(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<Contracts.Classes.Commands.CreateClassCommand> context)
  {
    var message = context.Message;
    var command = new CreateClassCommand
    {
      CourseId =  message.CourseId,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseStartDate = message.CourseStartDate,
      CourseEndDate = message.CourseEndDate,
      MaxStudents = message.MaxStudents,
    };
    var result = await _mediator.Send(command);

    if (result.IsError)
    {
      await context.Publish(new ClassCreateRejectionEvent
      {
        CourseId = message.CourseId,
        Reason = result.FirstError.Description,
        UserId = message.UserId,
      });
      return;
    }

    await context.Publish(new ClassCreatedEvent
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
