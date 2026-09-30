using System.Security.Claims;

namespace Platform.Common.Auth;

/// <summary>
/// Answers "who is this user?" and "is this user an administrator?" from the access token.
/// </summary>
public static class ClaimsPrincipalExtensions
{
  /// <summary>The claim that holds the user id in our access tokens (see <see cref="AccessTokenFactory"/>).</summary>
  public const string UserIdClaim = ClaimTypes.Sid;

  /// <summary>
  /// Returns the id of the signed-in user. Every access token we issue contains it, so a missing id means a bug
  /// (or an endpoint that forgot to require sign-in), not a user mistake.
  /// </summary>
  public static Guid GetUserId(this ClaimsPrincipal user) =>
    Guid.TryParse(user.FindFirstValue(UserIdClaim), out var userId)
      ? userId
      : throw new InvalidOperationException("The access token does not contain a user id.");

  /// <summary>
  /// True when the user has the administrator role. Looks at ALL role claims, not just the first one.
  /// </summary>
  public static bool IsAdministrator(this ClaimsPrincipal user) => user.IsInRole(Roles.Administrator);
}
