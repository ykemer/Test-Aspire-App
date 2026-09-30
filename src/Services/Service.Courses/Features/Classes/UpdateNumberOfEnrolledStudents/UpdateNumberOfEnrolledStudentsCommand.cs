namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public enum EnrollmentChange
{
  AddStudent,
  RemoveStudent
}

/// <param name="EventId">Id of the message that asked for this change. Used to apply the change only once.</param>
public record UpdateNumberOfEnrolledStudentsCommand(
  Guid EventId,
  Guid CourseId,
  Guid ClassId,
  EnrollmentChange Change)
  : IRequest<ErrorOr<Updated>>;
