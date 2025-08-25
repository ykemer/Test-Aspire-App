namespace Service.Students.Features.CreateStudent;

public class CreateStudentCommand : IRequest<ErrorOr<Created>>
{
  public required string Id { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
  public DateTime DateOfBirth { get; set; }
}
