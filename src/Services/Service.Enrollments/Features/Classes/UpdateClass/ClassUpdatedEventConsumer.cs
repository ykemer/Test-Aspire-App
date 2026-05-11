using Contracts.Classes.Events;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.UpdateClass;

public class ClassUpdatedEventConsumer : IHandleMessages<ClassUpdatedEvent>
{
  private readonly IMediator _mediator;

  public ClassUpdatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassUpdatedEvent message) =>
    await _mediator.Send(new UpdateClassCommand
    {
      MaxStudents = message.MaxStudents,
      CourseEndDate = message.CourseEndDate,
      CourseStartDate = message.CourseStartDate,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseId = message.CourseId,
      Id = message.Id
    });
}
