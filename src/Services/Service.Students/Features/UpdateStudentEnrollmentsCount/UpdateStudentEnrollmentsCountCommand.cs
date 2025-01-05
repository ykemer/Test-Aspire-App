namespace Service.Students.Features.UpdateStudentEnrollmentsCount;

public record UpdateStudentEnrollmentsCountCommand(string StudentId, bool Increase): IRequest<ErrorOr<Updated>>;