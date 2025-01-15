namespace Service.Students.Features.CreateStudent;

public class CreateStudentCommand : IRequest<ErrorOr<Created>>
{
  public string Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public DateTime DateOfBirth { get; set; }
}
