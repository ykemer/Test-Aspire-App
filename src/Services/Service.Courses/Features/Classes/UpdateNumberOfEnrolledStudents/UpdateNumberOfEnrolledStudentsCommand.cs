namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public record UpdateNumberOfEnrolledStudentsCommand(Guid CourseId, Guid ClassId, bool IsIncrease)
  : IRequest<ErrorOr<Updated>>;
