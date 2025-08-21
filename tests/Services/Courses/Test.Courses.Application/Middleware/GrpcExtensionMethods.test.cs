using CoursesGRPC;

using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;
using Service.Courses.Middleware;

namespace Courses.Application.Middleware;

[TestFixture]
public class GrpcExtensionMethodsTests
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

  [Test]
  public void MapToUpdateCourseCommand_MapsFields()
  {
    // Arrange
    var request = new GrpcUpdateCourseRequest { Id = "id-1", Name = "NewName", Description = "NewDesc" };

    // Act
    UpdateCourseCommand result = request.MapToUpdateCourseCommand();

    // Assert
    Assert.That(result.Id, Is.EqualTo("id-1"));
    Assert.That(result.Name, Is.EqualTo("NewName"));
    Assert.That(result.Description, Is.EqualTo("NewDesc"));
  }

  [Test]
  public void MapToDeleteCourseCommand_MapsId()
  {
    // Arrange
    var request = new GrpcDeleteCourseRequest { Id = "course-123" };

    // Act
    DeleteCourseCommand result = request.MapToDeleteCourseCommand();

    // Assert
    Assert.That(result.Id, Is.EqualTo("course-123"));
  }

  [Test]
  public void ToGetCourseQuery_MapsId()
  {
    // Arrange
    var request = new GrpcGetCourseRequest { Id = "course-abc" };

    // Act
    GetCourseQuery result = request.ToGetCourseQuery();

    // Assert
    Assert.That(result.Id, Is.EqualTo("course-abc"));
  }

  [Test]
  public void MapToListCoursesRequest_MapsPagingAndQuery()
  {
    // Arrange
    var request = new GrpcListCoursesRequest { Page = 3, PageSize = 20, Query = "search text" };

    // Act
    ListCoursesRequest result = request.MapToListCoursesRequest();

    // Assert
    Assert.That(result.PageNumber, Is.EqualTo(3));
    Assert.That(result.PageSize, Is.EqualTo(20));
    Assert.That(result.Query, Is.EqualTo("search text"));
  }
}
