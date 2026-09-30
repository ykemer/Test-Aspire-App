namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public sealed record UnenrollStudentFromClassCommand : IRequest<ErrorOr<Deleted>>
{
  public required Guid CourseId { get; init; }
  public required Guid ClassId { get; init; }
  public required Guid StudentId { get; init; }

  /// <summary>
  /// Chosen by the caller for this one attempt. Sending the same key again returns success without doing it twice.
  /// </summary>
  public required Guid IdempotencyKey { get; init; }
}
