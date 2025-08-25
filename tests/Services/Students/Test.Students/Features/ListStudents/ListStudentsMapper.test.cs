using Service.Students.Features.ListStudent;

using StudentsGRPC;

namespace Test.Students.Application.Features.ListStudents;

[TestFixture]
public class ListStudentsMapperTest
{
  [Test]
  public void MapToListStudentsQuery_MapsPaging()
  {
    // Arrange
    var request = new GrpcListStudentsRequest { Page = 2, PageSize = 50 };

    // Act
    var result = request.MapToListStudentsQuery();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(2));
    Assert.That(result.PageSize, Is.EqualTo(50));
  }
}
