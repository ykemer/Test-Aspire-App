using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Platform.Common.Database.Entities;

namespace Platform.Common.Auth;

public sealed record AccessToken(string Token, long ExpiresInSeconds);

public interface IAccessTokenFactory
{
  Task<AccessToken> CreateAsync(ApplicationUser user);
}

/// <summary>
/// Creates short-lived access tokens (JWT). They contain only the user id and roles, no personal data.
/// Uses the same library (and the same <see cref="JwtOptions"/>) that checks incoming tokens.
/// </summary>
public sealed class AccessTokenFactory : IAccessTokenFactory
{
  private static readonly JsonWebTokenHandler s_tokenHandler = new();

  private readonly JwtOptions _options;
  private readonly TimeProvider _timeProvider;
  private readonly UserManager<ApplicationUser> _userManager;

  public AccessTokenFactory(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options,
    TimeProvider timeProvider)
  {
    _userManager = userManager;
    _options = options.Value;
    _timeProvider = timeProvider;
  }

  public async Task<AccessToken> CreateAsync(ApplicationUser user)
  {
    var roles = await _userManager.GetRolesAsync(user);
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    var claims = new List<Claim> { new(ClaimsPrincipalExtensions.UserIdClaim, user.Id) };
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var token = s_tokenHandler.CreateToken(new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Issuer = _options.Issuer,
      Audience = _options.Audience,
      IssuedAt = now,
      NotBefore = now,
      Expires = now + JwtOptions.AccessTokenLifetime,
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SignKey)), SecurityAlgorithms.HmacSha256)
    });

    return new AccessToken(token, (long)JwtOptions.AccessTokenLifetime.TotalSeconds);
  }
}
