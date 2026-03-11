using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.GetStudent;

public record GetStudentQuery(Guid StudentId) : IRequest<ErrorOr<Student>>;
