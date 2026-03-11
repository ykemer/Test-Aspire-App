namespace Service.Students.Features.DeleteStudent;

public record DeleteStudentCommand(Guid StudentId) : IRequest<ErrorOr<Deleted>>;
