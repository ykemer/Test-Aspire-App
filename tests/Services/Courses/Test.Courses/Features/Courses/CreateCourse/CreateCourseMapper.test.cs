using CoursesGRPC;

using Service.Courses.Features.Courses.CreateCourse;

namespace Courses.Application.Features.Courses.CreateCourse;

[TestFixture]
public class CreateCourseMapperTest
{
  [Test]
  public void MapToCreateCourseCommand_MapsFields()
  {
    // Arrange
    var request = new GrpcCreateCourseRequest { Name = "Name", Description = "Desc" };

    // Act
    CreateCourseCommand result = request.MapToCreateCourseCommand();

    // Assert
    Assert.That(result.Name, Is.EqualTo("Name"));
    Assert.That(result.Description, Is.EqualTo("Desc"));
  }
}
