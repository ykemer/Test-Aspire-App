using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Features.Courses.DeleteCourse;

public class CourseDeletedEventConsumer: IConsumer<CourseDeletedEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseDeletedEventConsumer(IHubContext<CoursesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<CourseDeletedEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseDeleted,
        "Course deleted successfully.");
  }
}
