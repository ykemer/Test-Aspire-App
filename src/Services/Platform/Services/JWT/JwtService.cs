using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FastEndpoints.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using Platform.Entities;

namespace Platform.Services.JWT;

public class JwtService : IJwtService
{
  private readonly SignInManager<ApplicationUser> _signInManager;

  public JwtService(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;


  public async Task<JwtTokenServiceResponse> GenerateJwtToken(ApplicationUser user)
  {
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    var roles = await _signInManager.UserManager.GetRolesAsync(user);
    var jwtToken = JwtBearer.CreateToken(
      o =>
      {
        o.SigningKey = Environment.GetEnvironmentVariable("JWT_SIGN_KEY") ?? string.Empty;
        o.Issuer = Environment.GetEnvironmentVariable("JWT_KEY_ISSUER");
        o.Audience = Environment.GetEnvironmentVariable("JWT_KEY_AUDIENCE");
        o.ExpireAt = expiresAt;
        o.User.Claims.Add((ClaimTypes.Name, user.FirstName));
        o.User.Claims.Add((ClaimTypes.Surname, user.LastName));
        o.User.Claims.Add((ClaimTypes.Email, user.Email!));
        o.User.Claims.Add((ClaimTypes.Role, roles.FirstOrDefault()!));
        o.User.Claims.Add((ClaimTypes.Sid, user.Id));
        o.User.Roles.Add(roles.ToArray());
        o.User["UserId"] = user.Id; //indexer based claim setting
      });

    return new JwtTokenServiceResponse
    {
      AccessToken = jwtToken, ExpiresIn = (long)(expiresAt - DateTime.UtcNow).TotalSeconds
    };
  }

  private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
  {
    var secret = Environment.GetEnvironmentVariable("JWT_SIGN_KEY");

    TokenValidationParameters? validation = new()
    {
      ValidIssuer = "https+http://platform", // Environment.GetEnvironmentVariable("JWT_KEY_ISSUER"),
      ValidAudience = "https+http://platform", //Environment.GetEnvironmentVariable("JWT_KEY_AUDIENCE"),
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
      ValidateLifetime = false
    };

    return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
  }
}
