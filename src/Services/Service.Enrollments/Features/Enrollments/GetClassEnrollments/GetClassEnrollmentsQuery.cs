using Service.Enrollments.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetClassEnrollments;

public record GetClassEnrollmentsQuery(string CourseId, string ClassId) : IRequest<ErrorOr<List<Enrollment>>>;
