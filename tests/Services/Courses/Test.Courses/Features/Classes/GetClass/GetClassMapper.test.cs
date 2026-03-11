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
    var classId = Guid.NewGuid();
    var courseId = Guid.NewGuid();
    var enrolledClassId1 = Guid.NewGuid();
    var enrolledClassId2 = Guid.NewGuid();
    var request = new GrpcGetClassRequest { Id = classId.ToString(), CourseId = courseId.ToString(), ShowAll = true };
    request.EnrolledClasses.AddRange(new[] { enrolledClassId1.ToString(), enrolledClassId2.ToString() });

    // Act
    var query = request.ToGetClassQuery();

    // Assert
    Assert.That(query.Id, Is.EqualTo(classId));
    Assert.That(query.CourseId, Is.EqualTo(courseId));
    Assert.That(query.EnrolledClasses, Is.EquivalentTo(new[] { enrolledClassId1, enrolledClassId2 }));
    Assert.That(query.ShowAll, Is.True);
  }
}
