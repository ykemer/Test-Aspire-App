using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Contracts.Users.Requests;

public class UserRegisterRequest
{
  [Required]
  [MinLength(3)]
  [JsonPropertyName("firstName")]
  public string FirstName { get; set; }

  [Required]
  [MinLength(3)]
  [JsonPropertyName("lastName")]
  public string LastName { get; set; }

  [Required]
  [EmailAddress]
  [JsonPropertyName("email")]
  public string Email { get; set; }

  [Required]
  [PasswordPropertyText]
  [JsonPropertyName("password")]
  public string Password { get; set; }

  [Required]
  [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
  [JsonPropertyName("repeatPassword")]
  public string RepeatPassword { get; set; }

  [Required]
  [JsonPropertyName("dateOfBirth")]
  public DateTime DateOfBirth { get; set; }
}
