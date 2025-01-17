using System.Security.Claims;

using Aspire_App.Web.Services.Auth;

using Microsoft.AspNetCore.Components.Authorization;

namespace Aspire_App.Web.Handlers;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IAuthenticationService _authenticationService; // Service to interact with token storage

  public ApiAuthenticationStateProvider(IAuthenticationService authenticationService) =>
    _authenticationService = authenticationService;

  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var token = await _authenticationService.GetJwtAsync();
    if (string.IsNullOrEmpty(token))
    {
      return new AuthenticationState(new ClaimsPrincipal());
    }

    var userName = _authenticationService.GetUsername(token);
    var userRole = _authenticationService.GetUserRole(token);

    ClaimsIdentity? identity =
      new(new[] { new Claim(ClaimTypes.Name, userName), new Claim(ClaimTypes.Role, userRole) },
        "apiauth"); // Simplified

    ClaimsPrincipal? user = new(identity);
    return new AuthenticationState(user);
  }

  public void NotifyUserAuthentication()
  {
    ClaimsIdentity? identity = new(new[] { new Claim(ClaimTypes.Name, "User") }, "apiauth");
    ClaimsPrincipal? user = new(identity);
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
  }

  public void NotifyUserLogout()
  {
    ClaimsIdentity? identity = new();
    ClaimsPrincipal? user = new(identity);
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
  }
}
