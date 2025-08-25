using System.ComponentModel.DataAnnotations;

namespace Service.Enrollments.Database.Entities;

[Index(nameof(StudentId))]
[Index(nameof(CourseId))]
[Index(nameof(ClassId))]
public class Enrollment
{
  [Key] public string Id { get; init; } = Guid.CreateVersion7().ToString();

  public DateTime EnrollmentDateTime { get; set; } = DateTime.Now;

  [MaxLength(50)]
  public required string CourseId { get; set; }
  [MaxLength(50)]
  public required string ClassId { get; set; }
  [MaxLength(50)]
  public required string StudentId { get; set; }

  [MaxLength(50)]
  public required string StudentFirstName { get; set; }
  [MaxLength(50)]
  public required string StudentLastName { get; set; }

  public Class Class { get; set; }
}
