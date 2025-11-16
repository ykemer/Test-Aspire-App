using Contracts.Classes.Events;
using Contracts.Courses.Events;

using MassTransit;

namespace Service.Enrollments.Features.Classes.CreateClass;

public class ClassCreatedEventConsumer : IConsumer<ClassCreatedEvent>
{
  private readonly IMediator _mediator;

  public ClassCreatedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassCreatedEvent> context) =>
    await _mediator.Send(new CreateClassCommand
    {
      MaxStudents = context.Message.MaxStudents,
      CourseEndDate = context.Message.CourseEndDate,
      CourseStartDate = context.Message.CourseStartDate,
      RegistrationDeadline = context.Message.RegistrationDeadline,
      CourseId = context.Message.CourseId,
      Id = context.Message.Id
    });
}
