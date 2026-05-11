using Contracts.Classes.Events;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.CreateClass;

public class ClassCreatedEventConsumer : IHandleMessages<ClassCreatedEvent>
{
  private readonly IMediator _mediator;

  public ClassCreatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(ClassCreatedEvent message) =>
    await _mediator.Send(new CreateClassCommand
    {
      MaxStudents = message.MaxStudents,
      CourseEndDate = message.CourseEndDate,
      CourseStartDate = message.CourseStartDate,
      RegistrationDeadline = message.RegistrationDeadline,
      CourseId = message.CourseId,
      Id = message.Id
    });
}
