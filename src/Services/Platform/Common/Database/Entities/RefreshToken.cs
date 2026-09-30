namespace Platform.Common.Database.Entities;

/// <summary>
/// A long-lived token that lets a user get a new access token without typing the password again.
/// Only its hash is stored (see <see cref="Auth.RefreshTokenHasher"/>). Each token can be used once.
/// </summary>
public class RefreshToken
{
  public Guid Id { get; set; } = Guid.CreateVersion7();

  public required string TokenHash { get; set; }

  public required string UserId { get; set; }

  public ApplicationUser User { get; set; } = null!;

  public required DateTime CreatedAt { get; set; }

  public required DateTime ExpiresAt { get; set; }
}
