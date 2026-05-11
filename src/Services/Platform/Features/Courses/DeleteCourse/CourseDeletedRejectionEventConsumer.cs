using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.DeleteCourse;

public class CourseDeletedRejectionEventConsumer : IHandleMessages<CourseDeleteRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseDeletedRejectionEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseDeleteRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseDeleteRequestRejected, message.Reason);
}
