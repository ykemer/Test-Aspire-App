using Contracts.Classes.Events;

using MassTransit;

namespace Service.Enrollments.Features.Classes.ClassDeleted;

public class ClassDeletedEventConsumer : IConsumer<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassDeletedEvent> context) =>
    await _mediator.Send(new DeleteClassByClassIdCommand(context.Message.CourseId, context.Message.ClassId));
}
