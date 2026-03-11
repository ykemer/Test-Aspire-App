namespace Service.Courses.Common.Database.Entities;

public class Course
{
  public Guid Id { get; set; } = Guid.CreateVersion7();

  public required string Name { get; set; }

  public required string Description { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public int TotalStudents { get; set; }
  public IList<Class> CourseClasses { get; } = [];
}
