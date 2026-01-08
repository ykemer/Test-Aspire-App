using Contracts.Classes.Events;

using MassTransit;

namespace Service.Courses.Features.Classes.UpdateClass;

public class UpdateClassCommandConsumer : IConsumer<Contracts.Classes.Commands.UpdateClassCommand>
{
  private readonly IMediator _mediator;

  public UpdateClassCommandConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<Contracts.Classes.Commands.UpdateClassCommand> context)
  {
    var message = context.Message;
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
      await context.Publish(new ClassUpdateRejectionEvent
      {
        CourseId = message.CourseId,
        UserId = message.UserId,
        Reason = result.FirstError.Description,
        ClassId = message.ClassId
      });
      return;
    }

    await context.Publish(new ClassUpdatedEvent
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
