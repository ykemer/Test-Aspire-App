using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public record GetCourseEnrollmentsQuery(string CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
