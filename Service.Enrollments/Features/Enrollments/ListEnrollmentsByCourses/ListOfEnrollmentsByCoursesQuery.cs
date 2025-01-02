using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;

public record ListOfEnrollmentsByCoursesQuery(List<string> CourseIds, string? StudentId): IRequest<ErrorOr<List<Enrollment>>>;