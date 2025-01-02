namespace Service.Students.Features.DeleteStudent;

public record DeleteStudentCommand(string StudentId) : IRequest<ErrorOr<Deleted>>;
