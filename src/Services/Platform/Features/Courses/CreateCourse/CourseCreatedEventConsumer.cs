using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses.CreateCourse;

public class CourseCreatedEventConsumer: IConsumer<CourseCreatedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseCreatedEventConsumer(IHubContext<CoursesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<CourseCreatedEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseCreated,
        "Course created successfully.");
  }
}
