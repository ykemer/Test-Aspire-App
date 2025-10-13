using System.ComponentModel.DataAnnotations;

namespace Service.Courses.Common.Database.Entities;


[Index(nameof(CourseId))]
public class Class
{
  [Key] [Required] public string Id { get; set; } = Guid.CreateVersion7().ToString();
  [Required] public  required string CourseId { get; set; }

  [Required] public DateTime RegistrationDeadline { get; set; }
  [Required] public DateTime CourseStartDate { get; set; }
  [Required] public DateTime CourseEndDate { get; set; }
  [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  [Required] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public int MaxStudents { get; set; } = 0;

  public int TotalStudents { get; set; } = 0;
  public Course Course { get; set; }

}
