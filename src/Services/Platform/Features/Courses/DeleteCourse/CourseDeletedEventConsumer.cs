using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.DeleteCourse;

public class CourseDeletedEventConsumer : IHandleMessages<CourseDeletedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseDeletedEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseDeletedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseDeleted, "Course deleted successfully.");
}
