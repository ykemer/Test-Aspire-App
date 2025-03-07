namespace Service.Courses.Features.Courses.UpdateNumberOfEnrolledStudents;

public record UpdateNumberOfEnrolledStudentsCommand(string CourseId,  bool IsIncrease) : IRequest<ErrorOr<Updated>>;
