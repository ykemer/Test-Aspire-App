using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Platform.Entities;

public class ApplicationUser : IdentityUser
{
  [MaxLength(100)] public required string FirstName { get; set; }
  [MaxLength(100)] public required string LastName { get; set; }
  [MaxLength(100)] public string? RefreshToken { get; set; }
  public DateTime RefreshTokenExpiry { get; set; }
  public DateTime DateOfBirth { get; set; }
}
