using Microsoft.AspNetCore.SignalR;

namespace Platform.Features.Classes;

public class ClassesHub : Hub
{
  public async Task SendClassesNotification(string userId, string message) =>
    await Clients.User(userId).SendAsync("ReceiveClassesNotification", message);
}
