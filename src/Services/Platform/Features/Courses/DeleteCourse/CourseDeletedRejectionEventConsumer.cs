using Contracts.Courses.Events;
using Contracts.Courses.Hub;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses.DeleteCourse;

public class CourseDeletedRejectionEventConsumer : IConsumer<CourseDeleteRejectionEvent>
{
  private readonly IHubContext<CoursesHub> _hubContext;

  public CourseDeletedRejectionEventConsumer(IHubContext<CoursesHub> hubContext) => _hubContext = hubContext;

  public async Task Consume(ConsumeContext<CourseDeleteRejectionEvent> context) =>
    await _hubContext.Clients
      .User(context.Message.UserId).SendAsync(CourseHubMessage.CourseDeleteRequestRejected,
        context.Message.Reason);
}
