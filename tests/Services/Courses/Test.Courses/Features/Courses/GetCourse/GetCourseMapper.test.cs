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
    var courseId = Guid.NewGuid();
    var classGuid1 = Guid.NewGuid();
    var classGuid2 = Guid.NewGuid();
    var request =
      new GrpcGetCourseRequest { Id = courseId.ToString(), EnrolledClasses = { classGuid1.ToString(), classGuid2.ToString() }, ShowAll = true };

    // Act
    var result = request.ToGetCourseQuery();

    // Assert
    Assert.That(result.Id, Is.EqualTo(courseId));
    Assert.That(result.EnrolledClasses, Is.EquivalentTo(new[] { classGuid1, classGuid2 }));
    Assert.That(result.ShowAll, Is.True);
  }
}
