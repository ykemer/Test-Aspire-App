using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public record GetCourseEnrollmentsRequest(string CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
