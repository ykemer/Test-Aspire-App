using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Platform.Common.Database.Entities;

public class ApplicationUser : IdentityUser
{
  [MaxLength(100)] public required string FirstName { get; set; }
  [MaxLength(100)] public required string LastName { get; set; }
  public DateTime DateOfBirth { get; set; }
}
