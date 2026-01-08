using CoursesGRPC;

using Service.Courses.Features.Courses.GetCourse;

namespace Courses.Application.Features.Courses.GetCourse;

[TestFixture]
public class GetCourseMapperTest
{
  [Test]
  public void ToGetCourseQuery_MapsId()
  {
    // Arrange
    var request =
      new GrpcGetCourseRequest { Id = "course-abc", EnrolledClasses = { "class-1", "class-2" }, ShowAll = true };

    // Act
    var result = request.ToGetCourseQuery();

    // Assert
    Assert.That(result.Id, Is.EqualTo("course-abc"));
    Assert.That(result.EnrolledClasses, Is.EquivalentTo(new[] { "class-1", "class-2" }));
    Assert.That(result.ShowAll, Is.True);
  }
}
