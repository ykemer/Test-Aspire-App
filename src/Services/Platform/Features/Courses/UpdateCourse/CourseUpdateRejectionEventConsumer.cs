using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.UpdateCourse;

public class CourseUpdateRejectionEventConsumer : IHandleMessages<CourseUpdateRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseUpdateRejectionEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseUpdateRejectionEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseUpdateRequestRejected, message.Reason);
}
