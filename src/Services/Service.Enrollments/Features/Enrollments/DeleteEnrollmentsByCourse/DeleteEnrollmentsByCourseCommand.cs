namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByCourse;

public record DeleteEnrollmentsByCourseCommand(string CourseId) : IRequest<ErrorOr<Deleted>>;
