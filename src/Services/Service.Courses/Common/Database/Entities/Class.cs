namespace Service.Courses.Common.Database.Entities;

public class Class
{
  public Guid Id { get; set; } = Guid.CreateVersion7();
  public required Guid CourseId { get; set; }

  public DateTime RegistrationDeadline { get; set; }
  public DateTime CourseStartDate { get; set; }
  public DateTime CourseEndDate { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public int MaxStudents { get; set; }

  public int TotalStudents { get; set; }
  public Course Course { get; set; }
}
