using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.CreateCourse;

public class CourseCreateRejectionEventConsumer : IHandleMessages<CourseCreateRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseCreateRejectionEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseCreateRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseCreateRequestRejected, message.Reason);
}
