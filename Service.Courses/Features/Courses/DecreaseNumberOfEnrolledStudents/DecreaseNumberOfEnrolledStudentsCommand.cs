namespace Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;

public record DecreaseNumberOfEnrolledStudentsCommand(string CourseId) : IRequest<ErrorOr<Updated>>;
