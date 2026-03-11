namespace Service.Enrollments.Common.Database.Entities;

[Index(nameof(StudentId))]
[Index(nameof(CourseId))]
[Index(nameof(ClassId))]
public class Enrollment
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  public DateTime EnrollmentDateTime { get; set; } = DateTime.Now;

  public required Guid CourseId { get; set; }

  public required Guid ClassId { get; set; }

  public required Guid StudentId { get; set; }

  public required string StudentFirstName { get; set; }

  public required string StudentLastName { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public Class Class { get; set; }
}
