namespace Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;

public record EnrollStudentToCourseCommand(string CourseId, string StudentId, string FirstName, string LastName)
  : IRequest<ErrorOr<Created>>;
