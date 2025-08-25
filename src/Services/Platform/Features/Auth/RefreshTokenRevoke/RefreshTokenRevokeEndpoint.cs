using Contracts.Users.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Platform.Database;
using Platform.Database.Entities;

namespace Platform.Features.Auth.RefreshTokenRevoke;

public class RefreshTokenRevokeEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<Deleted>>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ApplicationDbContext _db;

  public RefreshTokenRevokeEndpoint(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public override void Configure()
  {
    Post("/api/auth/revoke");
    AllowAnonymous();
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(RefreshAccessTokenRequest req, CancellationToken ct)
  {

    var token =  await _db.RefreshTokens.FirstOrDefaultAsync(i => i.Token == req.RefreshToken, ct);
    if (token == null || token.ExpiresAt <= DateTime.UtcNow)
    {
      return Error.Unauthorized(description: "Refresh token is not valid");
    }

    _db.RefreshTokens.Remove(token);
    await _db.SaveChangesAsync(ct);
    return Result.Deleted;
  }
}
