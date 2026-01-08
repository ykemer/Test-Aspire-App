using CoursesGRPC;

using Service.Courses.Features.Courses.ListCourses;

namespace Courses.Application.Features.Courses.ListCourses;

[TestFixture]
public class ListCoursesMapperTest
{
  [Test]
  public void MapToListCoursesRequest_MapsPagingAndQuery()
  {
    // Arrange
    var request = new GrpcListCoursesRequest
    {
      Page = 3,
      PageSize = 20,
      Query = "search text",
      ShowAll = true,
      EnrolledClasses = { "course1" }
    };

    // Act
    var result = request.MapToListCoursesRequest();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(3));
    Assert.That(result.PageSize, Is.EqualTo(20));
    Assert.That(result.Query, Is.EqualTo("search text"));
    Assert.That(result.ShowAll, Is.True);
    Assert.That(result.EnrolledClasses, Is.EquivalentTo(new[] { "course1" }));
  }
}
