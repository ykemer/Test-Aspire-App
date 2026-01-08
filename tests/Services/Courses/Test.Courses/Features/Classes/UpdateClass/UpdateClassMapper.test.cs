using FizzWare.NBuilder;

using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.UpdateClass;

namespace Courses.Application.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassMapperTests
{
  [Test]
  public void AddCommandValues_ShouldUpdateEntityFields()
  {
    // Arrange

    var entity = Builder<Class>
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
