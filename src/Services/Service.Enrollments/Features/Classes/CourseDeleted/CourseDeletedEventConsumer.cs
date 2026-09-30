using Contracts.Courses.Events;

using Library.Messaging;

using Rebus.Handlers;

namespace Service.Enrollments.Features.Classes.CourseDeleted;

/// <summary>
/// Removes all classes of a deleted course. If that fails, the message is retried (never silently lost).
/// </summary>
public class CourseDeletedEventConsumer : IHandleMessages<CourseDeletedEvent>
{
  private readonly IMediator _mediator;

  public CourseDeletedEventConsumer(IMediator mediator) => _mediator = mediator;

  public async Task Handle(CourseDeletedEvent message)
  {
    var result = await _mediator.Send(new DeleteClassesByCourseIdCommand(message.CourseId));
    result.ThrowIfFailed($"Deleting classes of course {message.CourseId}");
  }
}
