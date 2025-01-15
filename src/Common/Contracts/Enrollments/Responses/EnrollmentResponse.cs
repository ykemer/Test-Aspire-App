namespace Contracts.Enrollments.Responses;

public class EnrollmentResponse
{
  public string Id { get; init; }

  public string CourseId { get; init; }
  public string StudentId { get; init; }
  public DateTime EnrollmentDateTime { get; init; }

  public string FirstName { get; init; }
  public string LastName { get; init; }
}
