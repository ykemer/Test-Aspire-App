using ClassesGRPC;

using Google.Protobuf.WellKnownTypes;

using Service.Courses.Features.Classes.CreateClass;

namespace Courses.Application.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassMapperTests
{
  [Test]
  public void MapToCreateClassCommand_MapsAllFields()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var request = new GrpcCreateClassRequest
    {
      CourseId = "course-1",
      RegistrationDeadline = now.ToTimestamp(),
      CourseStartDate = now.AddDays(1).ToTimestamp(),
      CourseEndDate = now.AddDays(2).ToTimestamp(),
      MaxStudents = 25
    };

    // Act
    var cmd = request.MapToCreateClassCommand();

    // Assert
    Assert.That(cmd.CourseId, Is.EqualTo("course-1"));
    Assert.That(cmd.RegistrationDeadline, Is.EqualTo(now).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.CourseStartDate, Is.EqualTo(now.AddDays(1)).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.CourseEndDate, Is.EqualTo(now.AddDays(2)).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.MaxStudents, Is.EqualTo(25));
  }
}
