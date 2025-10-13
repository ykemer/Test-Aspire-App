using System.ComponentModel.DataAnnotations;

namespace Service.Enrollments.Common.Database.Entities;

[Index(nameof(CourseId))]

public class Class
{
  [Key] public string Id { get; init; } = Guid.CreateVersion7().ToString();

  [MaxLength(50)]
  public required string CourseId { get; set; }

  public int MaxStudents { get; set; } = 0;

  [Required] public DateTime RegistrationDeadline { get; set; }
  [Required] public DateTime CourseStartDate { get; set; }
  [Required] public DateTime CourseEndDate { get; set; }


  public IList<Enrollment> Enrollments { get; set; } = [];
}
