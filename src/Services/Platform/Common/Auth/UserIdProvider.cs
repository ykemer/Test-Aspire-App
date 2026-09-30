using Microsoft.AspNetCore.SignalR;

namespace Platform.Common.Auth;

/// <summary>
/// Tells SignalR which user a connection belongs to, so hub messages can be sent to one user
/// with <c>Clients.User(userId)</c>. Uses the same user id claim as the access token.
/// </summary>
public class UserIdProvider : IUserIdProvider
{
  public string? GetUserId(HubConnectionContext connection)
  {
    var userIdClaim = connection.User?.FindFirst(ClaimsPrincipalExtensions.UserIdClaim)?.Value;
    return Guid.TryParse(userIdClaim, out var userId) ? userId.ToString() : null;
  }
}
