using CoursesGRPC;

using Service.Courses.Features.Courses.DeleteCourse;

namespace Courses.Application.Features.Courses.DeleteCourse;

[TestFixture]
public class DeleteCourseMapperTest
{
  [Test]
  public void MapToDeleteCourseCommand_MapsId()
  {
    // Arrange
    var request = new GrpcDeleteCourseRequest { Id = "course-123" };

    // Act
    var result = request.MapToDeleteCourseCommand();

    // Assert
    Assert.That(result.Id, Is.EqualTo("course-123"));
  }
}
