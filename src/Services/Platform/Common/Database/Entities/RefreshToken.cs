namespace Platform.Common.Database.Entities;

public class RefreshToken
{
  public Guid Id { get; set; } = Guid.CreateVersion7();

  public required string Token { get; set; }

  public required string UserId { get; set; }

  public ApplicationUser User { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime ExpiresAt { get; set; }

  public bool IsValid { get; set; } = true;
}
