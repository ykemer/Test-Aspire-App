using Service.Students.Features.ListStudents;

using StudentsGRPC;

namespace Test.Students.Features.ListStudents;

[TestFixture]
public class ListStudentsMapperTest
{
  [Test]
  public void ToListStudentsQuery_MapsPaging()
  {
    // Arrange
    var request = new GrpcListStudentsRequest { Page = 2, PageSize = 50 };

    // Act
    var result = request.ToListStudentsQuery();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(2));
    Assert.That(result.PageSize, Is.EqualTo(50));
  }
}
