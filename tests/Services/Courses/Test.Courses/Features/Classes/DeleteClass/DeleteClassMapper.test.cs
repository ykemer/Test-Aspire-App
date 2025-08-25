using ClassesGRPC;

using Service.Courses.Features.Classes.DeleteClass;

namespace Courses.Application.Features.Classes.DeleteClass;

[TestFixture]
public class DeleteClassMapperTests
{
  [Test]
  public void MapToDeleteClassCommand_MapsAllFields()
  {
    // Arrange
    var request = new GrpcDeleteClassRequest
    {
      Id = "class-1",
      CourseId = "course-1"
    };

    // Act
    var cmd = request.MapToDeleteClassCommand();

    // Assert
    Assert.That(cmd.Id, Is.EqualTo("class-1"));
    Assert.That(cmd.CourseId, Is.EqualTo("course-1"));
  }
}
