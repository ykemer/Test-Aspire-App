namespace Service.Students.Common.Database.Entities;

public class Student
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  public required string FirstName { get; init; }

  public required string LastName { get; init; }

  public required string Email { get; init; }

  public DateTime DateOfBirth { get; init; }

  public int EnrollmentsCount { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public override int GetHashCode() => HashCode.Combine(Id, FirstName, LastName, Email, DateOfBirth);
}
