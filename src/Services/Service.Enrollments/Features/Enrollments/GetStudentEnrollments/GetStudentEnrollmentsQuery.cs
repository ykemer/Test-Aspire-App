using Service.Enrollments.Common.Database.Entities;

namespace Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

public record GetStudentEnrollmentsQuery(string StudentId, string? CourseId) : IRequest<ErrorOr<List<Enrollment>>>;
