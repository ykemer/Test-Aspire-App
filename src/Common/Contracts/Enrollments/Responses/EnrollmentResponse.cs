namespace Contracts.Enrollments.Responses;

public class EnrollmentResponse
{
  public string Id { get; init; }

  public Guid CourseId { get; init; }
  public Guid StudentId { get; init; }
  public DateTime EnrollmentDateTime { get; init; }

  public string FirstName { get; init; }
  public string LastName { get; init; }
}
