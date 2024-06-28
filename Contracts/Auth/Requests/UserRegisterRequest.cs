using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Auth.Requests;


public class UserRegisterRequest
{
    [Required]
    [MinLength(3)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(3)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string RepeatPassword { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }
}