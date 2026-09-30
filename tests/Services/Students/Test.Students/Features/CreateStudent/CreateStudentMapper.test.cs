using Contracts.Users.Events;

using Service.Students.Features.CreateStudent;

using Test.Students.Setup;

namespace Test.Students.Features.CreateStudent;

[TestFixture]
public class CreateStudentMapperTests
{
  [Test]
  public void ToCreateStudentCommand_CopiesAllUserFields()
  {
    var userCreated = new UserCreatedEvent
    {
      Id = Guid.NewGuid(),
      FirstName = "First",
      LastName = "Last",
      Email = "some-email@email.com",
      DateOfBirth = new DateTime(2000, 1, 2)
    };

    var command = userCreated.ToCreateStudentCommand();

    Assert.Multiple(() =>
    {
      Assert.That(command.Id, Is.EqualTo(userCreated.Id));
      Assert.That(command.FirstName, Is.EqualTo(userCreated.FirstName));
      Assert.That(command.LastName, Is.EqualTo(userCreated.LastName));
      Assert.That(command.Email, Is.EqualTo(userCreated.Email));
      Assert.That(command.DateOfBirth, Is.EqualTo(userCreated.DateOfBirth));
    });
  }

  [Test]
  public void ToStudent_SetsTimestamps()
  {
    var command = new CreateStudentCommand
    {
      Id = Guid.NewGuid(),
      FirstName = "First",
      LastName = "Last",
      Email = "some-email@email.com",
      DateOfBirth = new DateTime(2000, 1, 2)
    };

    var student = command.ToStudent(TestClock.Now);

    Assert.That(student.Id, Is.EqualTo(command.Id));
    Assert.That(student.CreatedAt, Is.EqualTo(TestClock.Now));
    Assert.That(student.UpdatedAt, Is.EqualTo(TestClock.Now));
  }
}
