using Library.Auth;
using Library.Dates;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Platform.Common.Database;
using Platform.Common.Database.Entities;

namespace Platform.Common.Auth;

public interface IAuthTokenService
{
  /// <summary>Gives a signed-in user a new access token and a new refresh token.</summary>
  Task<AccessTokenResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken);

  /// <summary>Trades a refresh token for a new pair of tokens. The old refresh token stops working.</summary>
  Task<ErrorOr<AccessTokenResponse>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

  /// <summary>Makes a refresh token unusable (sign out).</summary>
  Task<ErrorOr<Deleted>> RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}

/// <summary>
/// Issues, rotates and revokes tokens.
/// <list type="bullet">
/// <item>Refresh tokens are stored only as a hash.</item>
/// <item>Each refresh token works once: using it deletes it with one atomic SQL DELETE, so two parallel
/// requests with the same token cannot both succeed.</item>
/// <item>Expired tokens of a user are cleaned up whenever that user gets new tokens.</item>
/// </list>
/// </summary>
public sealed class AuthTokenService : IAuthTokenService
{
  private readonly IAccessTokenFactory _accessTokenFactory;
  private readonly ApplicationDbContext _dbContext;
  private readonly TimeProvider _timeProvider;
  private readonly UserManager<ApplicationUser> _userManager;

  public AuthTokenService(IAccessTokenFactory accessTokenFactory, ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager, TimeProvider timeProvider)
  {
    _accessTokenFactory = accessTokenFactory;
    _dbContext = dbContext;
    _userManager = userManager;
    _timeProvider = timeProvider;
  }

  public async Task<AccessTokenResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
  {
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    await _dbContext.RefreshTokens
      .Where(token => token.UserId == user.Id && token.ExpiresAt <= now)
      .ExecuteDeleteAsync(cancellationToken);

    var refreshToken = Generators.GenerateToken();
    _dbContext.RefreshTokens.Add(new RefreshToken
    {
      TokenHash = RefreshTokenHasher.Hash(refreshToken),
      UserId = user.Id,
      CreatedAt = now,
      ExpiresAt = now + JwtOptions.RefreshTokenLifetime
    });
    await _dbContext.SaveChangesAsync(cancellationToken);

    var accessToken = await _accessTokenFactory.CreateAsync(user);
    return new AccessTokenResponse
    {
      AccessToken = accessToken.Token, ExpiresIn = accessToken.ExpiresInSeconds, RefreshToken = refreshToken
    };
  }

  public async Task<ErrorOr<AccessTokenResponse>> RefreshAsync(string refreshToken,
    CancellationToken cancellationToken)
  {
    var storedToken = await UseUpRefreshToken(refreshToken, cancellationToken);
    if (storedToken is null)
    {
      return AuthErrors.InvalidRefreshToken;
    }

    var user = await _userManager.FindByIdAsync(storedToken.UserId);
    if (user is null)
    {
      return AuthErrors.InvalidRefreshToken;
    }

    return await IssueTokensAsync(user, cancellationToken);
  }

  public async Task<ErrorOr<Deleted>> RevokeAsync(string refreshToken, CancellationToken cancellationToken)
  {
    var storedToken = await UseUpRefreshToken(refreshToken, cancellationToken);
    return storedToken is null ? AuthErrors.InvalidRefreshToken : Result.Deleted;
  }

  /// <summary>
  /// Finds a valid refresh token and deletes it in the same step.
  /// Returns null when the token is unknown, expired, or was just used by another request.
  /// </summary>
  private async Task<RefreshToken?> UseUpRefreshToken(string refreshToken, CancellationToken cancellationToken)
  {
    var tokenHash = RefreshTokenHasher.Hash(refreshToken);
    var storedToken = await _dbContext.RefreshTokens
      .AsNoTracking()
      .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    if (storedToken is null)
    {
      return null;
    }

    // Only one of two parallel requests can delete the row. The other one deletes nothing and fails.
    var deletedRows = await _dbContext.RefreshTokens
      .Where(token => token.Id == storedToken.Id)
      .ExecuteDeleteAsync(cancellationToken);
    if (deletedRows == 0)
    {
      return null;
    }

    var isExpired = storedToken.ExpiresAt.AsUtc() <= _timeProvider.GetUtcNow().UtcDateTime;
    return isExpired ? null : storedToken;
  }
}
