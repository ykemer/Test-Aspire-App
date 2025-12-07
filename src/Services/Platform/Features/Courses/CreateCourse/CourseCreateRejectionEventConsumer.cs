using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses.CreateCourse;

public class CourseCreateRejectionEventConsumer: IConsumer<CourseCreateRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseCreateRejectionEventConsumer(IHubContext<CoursesHub> hubContext)
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
