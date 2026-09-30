namespace Service.Enrollments.Common.Database.Entities;

/// <summary>
/// A local copy of a class owned by the Courses service, kept in sync through class events.
/// Enrollments needs it to check seats and deadlines without calling Courses.
/// </summary>
public class Class
{
  public Guid Id { get; init; } = Guid.CreateVersion7();

  public required Guid CourseId { get; set; }

  public int MaxStudents { get; set; }

  /// <summary>How many students are enrolled right now. Changed only with atomic SQL updates.</summary>
  public int EnrolledCount { get; set; }

  public DateTime RegistrationDeadline { get; set; }
  public DateTime CourseStartDate { get; set; }
  public DateTime CourseEndDate { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public IList<Enrollment> Enrollments { get; set; } = [];
}
