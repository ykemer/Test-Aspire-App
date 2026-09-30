namespace Platform.Common.Auth;

/// <summary>
/// Authorization policies used by the endpoints.
/// </summary>
public static class Policies
{
  /// <summary>Only administrators.</summary>
  public const string Administrators = "RequireAdministratorRole";

  /// <summary>Any signed-in user (students and administrators).</summary>
  public const string Users = "RequireUserRole";
}
