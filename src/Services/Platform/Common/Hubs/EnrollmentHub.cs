using Microsoft.AspNetCore.SignalR;

namespace Platform.Common.Hubs;

public class EnrollmentHub : Hub
{
  public async Task SendEnrollmentNotification(string userId, string message) =>
    await Clients.User(userId).SendAsync("ReceiveEnrollmentNotification", message);
}
