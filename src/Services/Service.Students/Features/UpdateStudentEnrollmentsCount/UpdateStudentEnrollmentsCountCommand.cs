namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public enum EnrollmentChange
{
  AddEnrollment,
  RemoveEnrollment
}

/// <param name="EventId">Id of the message that asked for this change. Used to apply the change only once.</param>
public record UpdateStudentEnrollmentsCountCommand(Guid EventId, Guid StudentId, EnrollmentChange Change)
  : IRequest<ErrorOr<Updated>>;
