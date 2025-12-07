using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses.UpdateCourse;

public class CourseUpdateRejectionEventConsumer: IConsumer<CourseUpdateRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseUpdateRejectionEventConsumer(IHubContext<CoursesHub> hubContext)
  {
    _hubContext = hubContext;
  }

  public async Task Consume(ConsumeContext<CourseUpdateRejectionEvent> context)
  {
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseUpdateRequestRejected,context.Message.Reason);
  }
}
