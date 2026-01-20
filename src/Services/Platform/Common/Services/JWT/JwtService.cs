using System.Security.Claims;

using FastEndpoints.Security;

using Microsoft.AspNetCore.Identity;

using Platform.Common.Database.Entities;

namespace Platform.Common.Services.JWT;

public class JwtService : IJwtService
{
  private readonly SignInManager<ApplicationUser> _signInManager;

  public JwtService(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;


  public async Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user)
  {
    var secret = Environment.GetEnvironmentVariable("JWT_SIGN_KEY")
                 ?? throw new InvalidOperationException("Missing JWT_SIGN_KEY");
    var issuer = Environment.GetEnvironmentVariable("JWT_KEY_ISSUER")
                 ?? throw new InvalidOperationException("Missing JWT_KEY_ISSUER");
    var audience = Environment.GetEnvironmentVariable("JWT_KEY_AUDIENCE")
                   ?? throw new InvalidOperationException("Missing JWT_KEY_AUDIENCE");

    var expiresAt = DateTime.UtcNow.AddMinutes(15);
    var roles = await _signInManager.UserManager.GetRolesAsync(user);

    var jwtToken = JwtBearer.CreateToken(o =>
    {
      o.SigningKey = secret;
      o.Issuer = issuer;
      o.Audience = audience;
      o.ExpireAt = expiresAt;

      // Minimal, non-PII claims: subject (user id) and roles
      o.User.Claims.Add(new Claim(ClaimTypes.Sid, user.Id));
      // Add roles as individual claims
      foreach (var r in roles)
      {
        o.User.Claims.Add(new Claim(ClaimTypes.Role, r));
      }
    });

    return new JwtTokenServiceResponse
    {
      AccessToken = jwtToken, ExpiresIn = (long)(expiresAt - DateTime.UtcNow).TotalSeconds
    };
  }
}
