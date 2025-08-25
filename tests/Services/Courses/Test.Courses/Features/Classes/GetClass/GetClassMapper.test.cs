using ClassesGRPC;

using Service.Courses.Features.Classes.GetClass;

namespace Courses.Application.Features.Classes.GetClass;

[TestFixture]
public class GetClassMapperTests
{
  [Test]
  public void ToGetClassQuery_MapsAllFields()
  {
    // Arrange
    var request = new GrpcGetClassRequest { Id = "class-1", CourseId = "course-1" , ShowAll = true};
    request.EnrolledClasses.AddRange(new[]{"c1","c2"});

    // Act
    var query = request.ToGetClassQuery();

    // Assert
    Assert.That(query.Id, Is.EqualTo("class-1"));
    Assert.That(query.CourseId, Is.EqualTo("course-1"));
    Assert.That(query.EnrolledClasses, Is.EquivalentTo(new[]{"c1","c2"}));
    Assert.That(query.ShowAll, Is.True);
  }
}
