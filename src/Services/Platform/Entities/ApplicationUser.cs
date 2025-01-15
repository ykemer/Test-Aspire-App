using Microsoft.AspNetCore.Identity;

namespace Platform.Entities;

public class ApplicationUser : IdentityUser
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string? RefreshToken { get; set; }
  public DateTime RefreshTokenExpiry { get; set; }
  public DateTime DateOfBirth { get; set; }
}
