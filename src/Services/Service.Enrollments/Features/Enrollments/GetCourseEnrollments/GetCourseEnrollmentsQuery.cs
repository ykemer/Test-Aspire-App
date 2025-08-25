using Service.Enrollments.Database.Entities;
using Service.Enrollments.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public record GetCourseEnrollmentsQuery(string CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
