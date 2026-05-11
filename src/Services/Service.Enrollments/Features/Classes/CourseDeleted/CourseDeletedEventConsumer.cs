using Contracts.Courses.Events;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.CourseDeleted;

public class CourseDeletedEventConsumer : IHandleMessages<CourseDeletedEvent>
{
  private readonly IMediator _mediator;

  public CourseDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(CourseDeletedEvent message) =>
    await _mediator.Send(new DeleteClassesByCourseIdCommand(message.CourseId));
}
