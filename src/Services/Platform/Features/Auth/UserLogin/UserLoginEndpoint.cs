using Contracts.Users.Requests;

using FastEndpoints;

using Library.Auth;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Entities;
using Platform.Services.JWT;

namespace Platform.Features.Auth.UserLogin;

public class UserLoginEndpoint : Endpoint<UserLoginRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly IJwtService _jwtService;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public UserLoginEndpoint(SignInManager<ApplicationUser> signInManager, IJwtService jwtService)
  {
    _signInManager = signInManager;
    _jwtService = jwtService;
  }

  public override void Configure()
  {
    Post("/api/auth/login");
    AllowAnonymous();
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserLoginRequest req, CancellationToken ct)
  {
    SignInResult? result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, false, false);
    if (!result.Succeeded)
    {
      return Error.Unauthorized(description: "Invalid email or password");
    }

    ApplicationUser? user = await _signInManager.UserManager.FindByEmailAsync(req.Email);


    JwtTokenServiceResponse? jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

    user.RefreshToken = Generators.GenerateToken();
    user.RefreshTokenExpiry = DateTime.Now.AddMonths(1);
    await _signInManager.UserManager.UpdateAsync(user);

    return new AccessTokenResponse
    {
      AccessToken = jwtTokenResponse.AccessToken,
      ExpiresIn = jwtTokenResponse.ExpiresIn,
      RefreshToken = user.RefreshToken ?? ""
    };
  }
}
