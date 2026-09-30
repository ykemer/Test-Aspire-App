namespace Platform.Common.Auth;

/// <summary>
/// The user roles that exist in the system.
/// </summary>
public static class Roles
{
  public const string Administrator = "Administrator";
  public const string User = "User";

  public static readonly string[] All = [Administrator, User];
}
