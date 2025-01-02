namespace Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

public record IncreaseNumberOfEnrolledStudentsCommand(string CourseId) : IRequest<ErrorOr<Updated>>;
