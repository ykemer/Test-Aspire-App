using Contracts.Classes.Events;
using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Classes.ClassDeleted;

namespace Service.Enrollments.Features.Classes.DeleteClass;

public class ClassDeletedEventConsumer : IConsumer<ClassDeletedEvent>
{
  private readonly IMediator _mediator;

  public ClassDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<ClassDeletedEvent> context) =>
    await _mediator.Send(new DeleteClassByClassIdCommand(context.Message.CourseId, context.Message.ClassId));
}
