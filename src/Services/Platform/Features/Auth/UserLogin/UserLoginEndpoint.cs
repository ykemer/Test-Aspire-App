using Contracts.Users.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

namespace Platform.Features.Auth.UserLogin;

/// <summary>
/// Signs a user in with email and password. After several wrong passwords the account is locked for a while
/// (ASP.NET Identity lockout). The error never says whether the email or the password was wrong.
/// </summary>
public class UserLoginEndpoint : Endpoint<UserLoginRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly IAuthTokenService _authTokens;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public UserLoginEndpoint(SignInManager<ApplicationUser> signInManager, IAuthTokenService authTokens)
  {
    _signInManager = signInManager;
    _authTokens = authTokens;
  }

  public override void Configure()
  {
    Post("/api/auth/login");
    AllowAnonymous();
    Description(x => x.WithTags("Auth"));
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserLoginRequest request,
    CancellationToken ct)
  {
    var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
    if (user is null)
    {
      return AuthErrors.InvalidCredentials;
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
    if (!result.Succeeded)
    {
      return AuthErrors.InvalidCredentials;
    }

    return await _authTokens.IssueTokensAsync(user, ct);
  }
}
