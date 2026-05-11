using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using Microsoft.AspNetCore.SignalR;

using Rebus.Handlers;

namespace Platform.Features.Courses.CreateCourse;

public class CourseCreatedEventConsumer : IHandleMessages<CourseCreatedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseCreatedEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Handle(CourseCreatedEvent message) =>
    await _hubContext.Clients
      .User(message.UserId).SendAsync(CourseHubMessage.CourseCreated, "Course created successfully.");
}
