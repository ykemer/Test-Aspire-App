using Service.Students.Features.GetStudent;

using StudentsGRPC;

namespace Test.Students.Features.GetStudent;

[TestFixture]
public class GetStudentMapperTest
{
  [Test]
  public void ToGetStudentQuery_MapsId()
  {
    // Arrange
    var id = Guid.NewGuid();
    var request = new GrpcGetStudentByIdRequest { Id = id.ToString() };

    // Act
    var result = request.ToGetStudentQuery();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo(id));
  }
}
