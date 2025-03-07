using System.ComponentModel.DataAnnotations;

namespace Service.Students.Entities;

public class Student
{
  [Key] public string Id { get; init; } = Guid.NewGuid().ToString();

  public string FirstName { get; init; }
  public string LastName { get; init; }
  public string Email { get; init; }

  public DateTime DateOfBirth { get; init; }

  public int EnrollmentsCount { get; set; } = 0;

  public override int GetHashCode()
  {
    return HashCode.Combine(Id, FirstName, LastName, Email, DateOfBirth);
  }
}
