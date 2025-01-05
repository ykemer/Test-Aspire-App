namespace Service.Enrollments.Features.Enrollments.DeleteEnrollmentsByStudent;

public record DeleteEnrollmentsByStudentCommand(string StudentId): IRequest<ErrorOr<Deleted>>;
