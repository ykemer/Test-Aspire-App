using System.ComponentModel.DataAnnotations;

namespace Service.Courses.Database.Entities;

public class Course
{
  [Key] [Required] public string Id { get; set; } = Guid.CreateVersion7().ToString();

  [Required]
  [MaxLength(100)]
  public required string Name { get; set; }

  [Required]
  [MaxLength(1000)]
  public required string Description { get; set; }

  [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public int TotalStudents { get; set; } = 0;
  public IList<Class> CourseClasses { get; } = [];
}
