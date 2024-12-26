using Microsoft.AspNetCore.Identity;

namespace Aspire_App.ApiService.Features.Auth;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime DateOfBirth { get; set; }
}