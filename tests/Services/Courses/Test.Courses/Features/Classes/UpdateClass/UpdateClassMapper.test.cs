using ClassesGRPC;

using FizzWare.NBuilder;

using Google.Protobuf.WellKnownTypes;

using Service.Courses.Database.Entities;
using Service.Courses.Features.Classes.UpdateClass;

namespace Courses.Application.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassMapperTests
{
  [Test]
  public void MapToUpdateClassCommand_MapsAllFields()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var request = new GrpcUpdateClassRequest
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = now.ToTimestamp(),
      CourseStartDate = now.AddDays(1).ToTimestamp(),
      CourseEndDate = now.AddDays(2).ToTimestamp(),
      MaxStudents = 50
    };

    // Act
    var cmd = request.MapToUpdateClassCommand();

    // Assert
    Assert.That(cmd.Id, Is.EqualTo("class-1"));
    Assert.That(cmd.CourseId, Is.EqualTo("course-1"));
    Assert.That(cmd.RegistrationDeadline, Is.EqualTo(now).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.CourseStartDate, Is.EqualTo(now.AddDays(1)).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.CourseEndDate, Is.EqualTo(now.AddDays(2)).Within(TimeSpan.FromSeconds(1)));
    Assert.That(cmd.MaxStudents, Is.EqualTo(50));
  }

  [Test]
  public void AddCommandValues_ShouldUpdateEntityFields()
  {
    // Arrange

    var entity =  Builder<Class>
      .CreateNew()
      .Build();

    var cmd = new UpdateClassCommand
    {
      Id = "class-1",
      CourseId = "course-1",
      RegistrationDeadline = DateTime.UtcNow.AddDays(-3),
      CourseStartDate = DateTime.UtcNow.AddDays(-2),
      CourseEndDate = DateTime.UtcNow.AddDays(-1),
      MaxStudents = 99
    };

    // Act
    entity.AddCommandValues(cmd);

    // Assert
    Assert.That(entity.CourseId, Is.EqualTo(cmd.CourseId));
    Assert.That(entity.RegistrationDeadline, Is.EqualTo(cmd.RegistrationDeadline));
    Assert.That(entity.CourseStartDate, Is.EqualTo(cmd.CourseStartDate));
    Assert.That(entity.CourseEndDate, Is.EqualTo(cmd.CourseEndDate));
    Assert.That(entity.MaxStudents, Is.EqualTo(cmd.MaxStudents));
  }
}
