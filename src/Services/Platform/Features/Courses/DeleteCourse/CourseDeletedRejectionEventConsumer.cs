using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Platform.Common.Hubs;

namespace Platform.Features.Courses.DeleteCourse;

public class CourseDeletedRejectionEventConsumer: IConsumer<CourseCreateRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseDeletedRejectionEventConsumer(IHubContext<CoursesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<CourseCreateRejectionEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseCreateRequestRejected,
        context.Message.Reason);
  }
}
