using Contracts.Users.Requests;

using FastEndpoints;

using Library.Auth;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Common.Database;
using Platform.Common.Database.Entities;
using Platform.Common.Services.JWT;

namespace Platform.Features.Auth.UserLogin;

public class UserLoginEndpoint : Endpoint<UserLoginRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly ApplicationDbContext _db;
  private readonly IJwtService _jwtService;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public UserLoginEndpoint(SignInManager<ApplicationUser> signInManager, IJwtService jwtService,
    ApplicationDbContext db)
  {
    _signInManager = signInManager;
    _jwtService = jwtService;
    _db = db;
  }

  public override void Configure()
  {
    Post("/api/auth/login");
    AllowAnonymous();

    Description(x => x.WithTags("Auth"));
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserLoginRequest req, CancellationToken ct)
  {
    var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, false, false);
    if (!result.Succeeded)
    {
      return Error.Unauthorized(description: "Invalid email or password");
    }

    var user = await _signInManager.UserManager.FindByEmailAsync(req.Email);


    var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

    var refreshToken = new RefreshToken
    {
      Token = Generators.GenerateToken(), ExpiresAt = DateTime.Now.AddDays(7), UserId = user.Id
    };

    await _signInManager.UserManager.UpdateAsync(user);
    await _db.RefreshTokens.AddAsync(refreshToken, ct);
    await _db.SaveChangesAsync(ct);

    return new AccessTokenResponse
    {
      AccessToken = jwtTokenResponse.AccessToken,
      ExpiresIn = jwtTokenResponse.ExpiresIn,
      RefreshToken = refreshToken.Token
    };
  }
}
