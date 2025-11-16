using Contracts.Courses.Events;

using MassTransit;

using Service.Enrollments.Features.Classes.CourseDeleted;

namespace Service.Enrollments.Features.Courses.DeleteCourse;

public class CourseDeletedEventConsumer : IConsumer<CourseDeletedEvent>
{
  private readonly IMediator _mediator;

  public CourseDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Consume(ConsumeContext<CourseDeletedEvent> context) =>
    await _mediator.Send(new DeleteClassesByCourseIdCommand(context.Message.CourseId));
}
