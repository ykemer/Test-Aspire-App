using Service.Students.Features.GetStudent;

using StudentsGRPC;

namespace Test.Students.Application.Features.GetStudent;

[TestFixture]
public class GetStudentMapperTest
{
  [Test]
  public void MapToGetStudentQuery_MapsId()
  {
    // Arrange
    var id = Guid.NewGuid();
    var request = new GrpcGetStudentByIdRequest { Id = id.ToString() };

    // Act
    var result = request.MapToGetStudentQuery();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo(id));
  }
}
