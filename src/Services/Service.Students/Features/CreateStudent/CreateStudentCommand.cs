namespace Service.Students.Features.CreateStudent;

public sealed record CreateStudentCommand : IRequest<ErrorOr<Created>>
{
  public required Guid Id { get; init; }
  public required string FirstName { get; init; }
  public required string LastName { get; init; }
  public required string Email { get; init; }
  public required DateTime DateOfBirth { get; init; }
}
