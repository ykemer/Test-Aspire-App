using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Courses;

public class CoursesHub : Hub
{
  public async Task SendCoursesNotification(string userId, string message) =>
    await Clients.User(userId).SendAsync("ReceiveCoursesNotification", message);
}
