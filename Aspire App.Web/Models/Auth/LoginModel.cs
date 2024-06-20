using System.ComponentModel.DataAnnotations;

namespace Aspire_App.Web.Models.Auth;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}