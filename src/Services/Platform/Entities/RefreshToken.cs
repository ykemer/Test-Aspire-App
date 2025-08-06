using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Entities;

public class RefreshToken
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  [Required]
  public string Token { get; set; } = string.Empty;

  [Required]
  public string UserId { get; set; }

  [ForeignKey(nameof(UserId))]
  public ApplicationUser User { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime ExpiresAt { get; set; }

  public bool IsValid { get; set; } = true;
}
