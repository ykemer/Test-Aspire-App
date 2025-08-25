namespace Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;

public record UpdateNumberOfEnrolledStudentsCommand(string CourseId, string ClassId,  bool IsIncrease) : IRequest<ErrorOr<Updated>>;
