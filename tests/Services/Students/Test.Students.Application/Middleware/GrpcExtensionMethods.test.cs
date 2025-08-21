using StudentsGRPC;

using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;
using Service.Students.Middleware;

namespace Test.Students.Application.Middleware;

[TestFixture]
public class GrpcExtensionMethodsTests
{
  [Test]
  public void MapToGetStudentQuery_MapsId()
  {
    // Arrange
    var request = new GrpcGetStudentByIdRequest { Id = "stud-123" };

    // Act
    GetStudentQuery result = request.MapToGetStudentQuery();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo("stud-123"));
  }

  [Test]
  public void MapToListStudentsQuery_MapsPaging()
  {
    // Arrange
    var request = new GrpcListStudentsRequest { Page = 2, PageSize = 50 };

    // Act
    ListStudentsQuery result = request.MapToListStudentsQuery();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(2));
    Assert.That(result.PageSize, Is.EqualTo(50));
  }

  [Test]
  public void MapToDeleteStudentCommand_MapsId()
  {
    // Arrange
    var request = new GrpcDeleteStudentRequest { Id = "stud-xyz" };

    // Act
    DeleteStudentCommand result = request.MapToDeleteStudentCommand();

    // Assert
    Assert.That(result.StudentId, Is.EqualTo("stud-xyz"));
  }
}
