using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.UpdateCourse;

public class CourseUpdatedEventConsumer : IHandleMessages<CourseUpdatedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseUpdatedEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseUpdatedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseUpdated, "Course updated successfully.");
}
