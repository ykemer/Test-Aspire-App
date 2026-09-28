namespace Contracts.Enrollments.Requests;

public class ChangeCourseEnrollmentRequest
{
  public Guid StudentId { get; set; } = Guid.Empty;

  public Guid IdempotencyKey { get; set; } = Guid.NewGuid();
}
