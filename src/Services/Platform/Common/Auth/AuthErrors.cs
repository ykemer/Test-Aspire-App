using Library.GRPC;

namespace Platform.Common.Auth;

/// <summary>
/// Every error the sign-in endpoints can return. Messages are deliberately vague where
/// a precise message would help an attacker (for example "wrong password" vs "no such user").
/// </summary>
public static class AuthErrors
{
  public static readonly Error InvalidCredentials =
    Error.Unauthorized("platform.auth.invalid_credentials", "Invalid email or password.");

  public static readonly Error InvalidRefreshToken =
    Error.Unauthorized("platform.auth.invalid_refresh_token", "The refresh token is not valid. Please sign in again.");

  public static readonly Error EmailAlreadyRegistered =
    ConflictErrors.AlreadyExists("platform.auth.email_already_registered",
      "An account with this email already exists.");
}
