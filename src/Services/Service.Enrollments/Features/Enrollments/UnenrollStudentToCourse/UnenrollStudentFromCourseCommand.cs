namespace Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

public record UnenrollStudentFromCourseCommand(string CourseId, string StudentId) : IRequest<ErrorOr<Deleted>>;
