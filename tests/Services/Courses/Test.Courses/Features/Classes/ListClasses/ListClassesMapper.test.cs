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
    var id = Guid.NewGuid();
    var enrolledClassId1 = Guid.NewGuid();
    var enrolledClassId2 = Guid.NewGuid();
    var request = new GrpcListClassRequest { Page = 2, PageSize = 15, CourseId = id.ToString(), ShowAll = true };
    request.EnrolledClasses.AddRange(new[] { enrolledClassId1.ToString(), enrolledClassId2.ToString() });

    // Act
    var result = request.MapToListClassesRequest();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(2));
    Assert.That(result.PageSize, Is.EqualTo(15));
    Assert.That(result.CourseId, Is.EqualTo(id));
    Assert.That(result.EnrolledClasses, Is.EquivalentTo(new[] { enrolledClassId1, enrolledClassId2 }));
    Assert.That(result.ShowAll, Is.True);
  }
}
