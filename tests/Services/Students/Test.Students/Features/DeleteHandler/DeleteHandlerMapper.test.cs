using Service.Students.Features.DeleteStudent;

using StudentsGRPC;

namespace Test.Students.Application.Features.DeleteHandler;

[TestFixture]
public class DeleteHandlerMapperTest
{
  [Test]
  public void MapToDeleteStudentCommand_MapsId()
  {
    // Arrange
    var id = Guid.NewGuid();
    var request = new GrpcDeleteStudentRequest { Id = id.ToString() };

    // Act
    var result = request.MapToDeleteStudentCommand();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo(id));
  }
}
