using Contracts.Users.Requests;

using FastEndpoints;

using Library.Auth;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Platform.Common.Database;
using Platform.Common.Database.Entities;
using Platform.Common.Services.JWT;

namespace Platform.Features.Auth.RefreshAccessToken;

public class RefreshAccessTokenEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly ApplicationDbContext _db;
  private readonly IJwtService _jwtService;
  private readonly UserManager<ApplicationUser> _userManager;

  public RefreshAccessTokenEndpoint(IJwtService jwtService, UserManager<ApplicationUser> userManager,
    ApplicationDbContext db)
  {
    _jwtService = jwtService;
    _userManager = userManager;
    _db = db;
  }

  public override void Configure()
  {
    Post("/api/auth/refresh");
    AllowAnonymous();
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(RefreshAccessTokenRequest req,
    CancellationToken ct)
  {
    var token = await _db.RefreshTokens.FirstOrDefaultAsync(i => i.Token == req.RefreshToken, ct);
    if (token == null || token.ExpiresAt <= DateTime.UtcNow)
    {
      return Error.Unauthorized(description: "Refresh token is not valid");
    }

    var user = await _userManager.FindByIdAsync(token.UserId);
    if (user == null)
    {
      return Error.Unauthorized(description: "User not found");
    }

    var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);
    var newToken = new RefreshToken
    {
      Token = Generators.GenerateToken(), ExpiresAt = DateTime.Now.AddDays(7), UserId = user.Id
    };

    _db.RefreshTokens.Remove(token);
    await _db.RefreshTokens.AddAsync(newToken, ct);
    await _db.SaveChangesAsync(ct);

    return new AccessTokenResponse
    {
      AccessToken = jwtTokenResponse.AccessToken,
      ExpiresIn = jwtTokenResponse.ExpiresIn,
      RefreshToken = newToken.Token
    };
  }
}
