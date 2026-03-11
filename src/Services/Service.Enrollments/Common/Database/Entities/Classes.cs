using System.ComponentModel.DataAnnotations;

namespace Service.Enrollments.Common.Database.Entities;

public class Class
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  public required Guid CourseId { get; set; }

  public int MaxStudents { get; set; } = 0;

  public DateTime RegistrationDeadline { get; set; }
  public DateTime CourseStartDate { get; set; }
  public DateTime CourseEndDate { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }


  public IList<Enrollment> Enrollments { get; set; } = [];
}
