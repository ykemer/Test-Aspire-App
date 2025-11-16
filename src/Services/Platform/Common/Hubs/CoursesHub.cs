using Microsoft.AspNetCore.SignalR;

namespace Platform.Common.Hubs;

public class CoursesHub : Hub
{
  public async Task SendCoursesNotification(string userId, string message) =>
    await Clients.User(userId).SendAsync("ReceiveCoursesNotification", message);
}
