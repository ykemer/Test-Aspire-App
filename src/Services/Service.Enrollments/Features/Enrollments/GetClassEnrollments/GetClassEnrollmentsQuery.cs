using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public record GetClassEnrollmentsQuery(Guid CourseId, Guid ClassId) : IRequest<ErrorOr<List<Enrollment>>>;
