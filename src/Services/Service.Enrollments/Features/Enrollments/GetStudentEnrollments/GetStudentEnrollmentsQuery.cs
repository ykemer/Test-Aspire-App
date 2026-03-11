using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public record GetStudentEnrollmentsQuery(Guid StudentId, Guid? CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
