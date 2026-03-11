using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

public record GetCourseEnrollmentsQuery(Guid CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
