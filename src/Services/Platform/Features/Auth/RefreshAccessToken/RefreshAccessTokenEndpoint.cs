using Contracts.Users.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Entities;
using Platform.Services.JWT;

namespace Platform.Features.Auth.RefreshAccessToken;

public class RefreshAccessTokenEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly IJwtService _jwtService;
  private readonly UserManager<ApplicationUser> _userManager;

  public RefreshAccessTokenEndpoint(IJwtService jwtService, UserManager<ApplicationUser> userManager)
  {
    _jwtService = jwtService;
    _userManager = userManager;
  }

  public override void Configure()
  {
    Post("/api/auth/refresh");
    AllowAnonymous();
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(RefreshAccessTokenRequest req,
    CancellationToken ct)
  {
    var user = _userManager.Users.FirstOrDefault(i => i.RefreshToken == req.RefreshToken);
    if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
    {
      return Error.Unauthorized(description: "Refresh token is not valid");
    }

    var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

    return new AccessTokenResponse
    {
      AccessToken = jwtTokenResponse.AccessToken,
      ExpiresIn = jwtTokenResponse.ExpiresIn,
      RefreshToken = user.RefreshToken ?? ""
    };
  }
}
