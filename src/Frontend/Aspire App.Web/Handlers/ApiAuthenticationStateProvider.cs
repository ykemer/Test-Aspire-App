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

    var userId = _authenticationService.GetUserId(token);
    var userRole = _authenticationService.GetUserRole(token);

    var identity =
      new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, userId), new Claim(ClaimTypes.Role, userRole) },
        "apiauth"); // Simplified

    var user = new ClaimsPrincipal(identity);
    return new AuthenticationState(user);
  }
}
