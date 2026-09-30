namespace Service.Students.Common.Database.Entities;

/// <summary>
/// A student. Created from the "user created" event sent by Platform when someone registers.
/// </summary>
public class Student
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  public required string FirstName { get; init; }

  public required string LastName { get; init; }

  public required string Email { get; init; }

  public DateTime DateOfBirth { get; init; }

  /// <summary>How many classes the student is enrolled in. Changed only with atomic SQL updates.</summary>
  public int EnrollmentsCount { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
