namespace Service.Enrollments.Common.Database.Entities;

/// <summary>
/// One student taking one class.
/// </summary>
public class Enrollment
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  /// <summary>When the student enrolled (UTC).</summary>
  public DateTime EnrollmentDateTime { get; set; }

  public required Guid CourseId { get; set; }

  public required Guid ClassId { get; set; }

  public required Guid StudentId { get; set; }

  public required string StudentFirstName { get; set; }

  public required string StudentLastName { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public Class Class { get; set; } = null!;
}
