using Service.Students.Features.CreateStudent;

namespace Test.Students.Application.Features.CreateStudent;

[TestFixture]
public class CreateStudentMapperTest
{
  [Test]
  public void MapToCreateStudentCommand_MapsFields()
  {
    var command = new CreateStudentCommand
    {
      Id = "id-1",
      FirstName = "First",
      LastName = "Last",
      Email = "some-email@email.com",
      DateOfBirth = DateTime.UtcNow
    };

    var output = command.MapToStudent();
    Assert.That(output.Id, Is.EqualTo(command.Id));
    Assert.That(output.FirstName, Is.EqualTo(command.FirstName));
    Assert.That(output.LastName, Is.EqualTo(command.LastName));
    Assert.That(output.Email, Is.EqualTo(command.Email));
    Assert.That(output.DateOfBirth, Is.EqualTo(command.DateOfBirth));
  }
}
