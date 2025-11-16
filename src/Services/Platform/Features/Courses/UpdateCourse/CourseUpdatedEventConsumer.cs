using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Features.Courses.UpdateCourse;

public class CourseUpdatedEventConsumer: IConsumer<CourseUpdatedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseUpdatedEventConsumer(IHubContext<CoursesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<CourseUpdatedEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseUpdated,
        "Course updated successfully.");
  }
}
