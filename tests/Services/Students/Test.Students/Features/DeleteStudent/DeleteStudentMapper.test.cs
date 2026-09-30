using Service.Students.Features.DeleteStudent;

using StudentsGRPC;

namespace Test.Students.Features.DeleteStudent;

[TestFixture]
public class DeleteStudentMapperTest
{
  [Test]
  public void ToDeleteStudentCommand_MapsId()
  {
    // Arrange
    var id = Guid.NewGuid();
    var request = new GrpcDeleteStudentRequest { Id = id.ToString() };

    // Act
    var result = request.ToDeleteStudentCommand();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo(id));
  }
}
