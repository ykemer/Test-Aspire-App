using Microsoft.AspNetCore.Identity;

namespace Platform.Common.Auth;

/// <summary>
/// The password policy. Used to configure ASP.NET Identity AND by the register validator,
/// so the user gets a clear validation message instead of a failure from Identity.
/// </summary>
public static class PasswordRules
{
  public const int MinLength = 8;
  public const int MaxLength = 100;

  public static void ApplyTo(PasswordOptions options)
  {
    options.RequiredLength = MinLength;
    options.RequireDigit = true;
    options.RequireLowercase = true;
    options.RequireUppercase = true;
    options.RequireNonAlphanumeric = true;
  }
}
