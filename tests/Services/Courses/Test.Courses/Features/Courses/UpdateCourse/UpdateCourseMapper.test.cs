using CoursesGRPC;

using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application.Features.Courses.UpdateCourse;

[TestFixture]
public class UpdateCourseMapperTest
{
  [Test]
  public void MapToUpdateCourseCommand_MapsFields()
  {
    // Arrange
    var request = new GrpcUpdateCourseRequest { Id = "id-1", Name = "NewName", Description = "NewDesc" };

    // Act
    var result = request.MapToUpdateCourseCommand();

    // Assert
    Assert.That(result.Id, Is.EqualTo("id-1"));
    Assert.That(result.Name, Is.EqualTo("NewName"));
    Assert.That(result.Description, Is.EqualTo("NewDesc"));
  }
}
