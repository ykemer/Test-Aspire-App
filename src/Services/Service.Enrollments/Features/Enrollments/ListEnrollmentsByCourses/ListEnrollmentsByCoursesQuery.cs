using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;

public record ListEnrollmentsByCoursesQuery(List<string> CourseIds, string? StudentId)
  : IRequest<ErrorOr<List<Enrollment>>>;
