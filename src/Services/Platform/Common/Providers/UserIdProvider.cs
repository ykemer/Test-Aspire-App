using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;

namespace Platform.Common.Providers;

public class UserIdProvider : IUserIdProvider
{
  public string? GetUserId(HubConnectionContext connection)
  {
    var sidClaim = connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
    return Guid.TryParse(sidClaim, out var id) ? id.ToString() : null;
  }
}
