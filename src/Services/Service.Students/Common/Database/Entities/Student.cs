using System.ComponentModel.DataAnnotations;

namespace Service.Students.Common.Database.Entities;

public class Student
{
  [Key] public string Id { get; init; } = Guid.CreateVersion7().ToString();

  [MaxLength(100)]
  public required string FirstName { get; init; }
  [MaxLength(100)]
  public required string LastName { get; init; }
  [MaxLength(100)]
  [EmailAddress]
  public required string Email { get; init; }

  public DateTime DateOfBirth { get; init; }

  public int EnrollmentsCount { get; set; } = 0;

  public override int GetHashCode()
  {
    return HashCode.Combine(Id, FirstName, LastName, Email, DateOfBirth);
  }
}
