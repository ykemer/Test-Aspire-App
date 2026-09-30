using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Platform.Common.Auth;

/// <summary>
/// Settings for signing and checking access tokens. Read from the JWT_* environment variables
/// (in development they come from the .env file). The service refuses to start if they are missing or weak.
/// </summary>
public sealed class JwtOptions
{
  public const string SignKeySetting = "JWT_SIGN_KEY";
  public const string IssuerSetting = "JWT_KEY_ISSUER";
  public const string AudienceSetting = "JWT_KEY_AUDIENCE";

  /// <summary>HMAC-SHA256 needs a key of at least 256 bits, i.e. 32 characters.</summary>
  public const int MinSignKeyLength = 32;

  public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
  public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

  [Required(ErrorMessage = $"{SignKeySetting} is missing.")]
  [MinLength(MinSignKeyLength, ErrorMessage = $"{SignKeySetting} must be at least 32 characters long.")]
  public string SignKey { get; set; } = string.Empty;

  [Required(ErrorMessage = $"{IssuerSetting} is missing.")]
  public string Issuer { get; set; } = string.Empty;

  [Required(ErrorMessage = $"{AudienceSetting} is missing.")]
  public string Audience { get; set; } = string.Empty;

  public void ReadFrom(IConfiguration configuration)
  {
    SignKey = configuration[SignKeySetting] ?? string.Empty;
    Issuer = configuration[IssuerSetting] ?? string.Empty;
    Audience = configuration[AudienceSetting] ?? string.Empty;
  }

  /// <summary>
  /// The rules used to accept an incoming access token. Kept here, next to the settings used to create tokens,
  /// so creating and checking can never drift apart.
  /// </summary>
  public TokenValidationParameters CreateValidationParameters() =>
    new()
    {
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SignKey)),
      ValidateIssuerSigningKey = true,
      ValidIssuer = Issuer,
      ValidateIssuer = true,
      ValidAudience = Audience,
      ValidateAudience = true,
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    };
}
