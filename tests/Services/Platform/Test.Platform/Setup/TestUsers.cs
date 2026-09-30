using System.Security.Claims;

using Platform.Common.Auth;

namespace Test.Platform.Setup;

/// <summary>
/// Builds signed-in users the way our access tokens describe them (user id claim + role claims).
/// </summary>
public static class TestUsers
{
  public static ClaimsPrincipal Student(Guid? id = null) => WithRoles(id ?? Guid.NewGuid(), Roles.User);

  public static ClaimsPrincipal Administrator(Guid? id = null) =>
    WithRoles(id ?? Guid.NewGuid(), Roles.User, Roles.Administrator);

  public static ClaimsPrincipal WithRoles(Guid id, params string[] roles)
  {
    var claims = new List<Claim> { new(ClaimsPrincipalExtensions.UserIdClaim, id.ToString()) };
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Test"));
  }
}
