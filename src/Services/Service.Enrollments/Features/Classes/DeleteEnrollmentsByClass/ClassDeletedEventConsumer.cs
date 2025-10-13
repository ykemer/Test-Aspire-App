using Contracts.Courses.Events;

using MassTransit;

namespace Service.Enrollments.Features.Classes.DeleteEnrollmentsByClass;

public class ClassDeletedEventConsumer : IConsumer<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassDeletedEvent> context) =>
    await _mediator.Send(new DeleteEnrollmentsByClassCommand(context.Message.CourseId, context.Message.ClassId));
}
