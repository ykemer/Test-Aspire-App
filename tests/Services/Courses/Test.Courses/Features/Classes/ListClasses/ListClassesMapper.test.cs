using ClassesGRPC;

using Service.Courses.Features.Classes.ListClasses;

namespace Courses.Application.Features.Classes.ListClasses;

[TestFixture]
public class ListClassesMapperTests
{
  [Test]
  public void MapToListClassesRequest_MapsPagingAndCourseIdAndEnrolled()
  {
    // Arrange
    var request = new GrpcListClassRequest { Page = 2, PageSize = 15, CourseId = "course-1" , ShowAll = true};
    request.EnrolledClasses.AddRange(new []{"c1","c2"});

    // Act
    var result = request.MapToListClassesRequest();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(2));
    Assert.That(result.PageSize, Is.EqualTo(15));
    Assert.That(result.CourseId, Is.EqualTo("course-1"));
    Assert.That(result.EnrolledClasses, Is.EquivalentTo(new []{"c1","c2"}));
    Assert.That(result.ShowAll, Is.True);
  }
}
