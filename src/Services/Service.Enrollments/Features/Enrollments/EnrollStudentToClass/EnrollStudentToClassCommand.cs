namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public sealed record EnrollStudentToClassCommand : IRequest<ErrorOr<Created>>
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }

  /// <summary>
  /// Chosen by the caller for this one attempt. Sending the same key again returns success without enrolling twice.
  /// </summary>
  public required Guid IdempotencyKey { get; init; }
}
