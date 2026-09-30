using FizzWare.NBuilder;

using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.UpdateClass;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassMapperTests
{
  [Test]
  public void ApplyUpdate_ShouldCopyEditableFields_AndKeepCourse()
  {
    // Arrange
    var entity = Builder<Class>.CreateNew().Build();
    var originalCourseId = entity.CourseId;
    var command = new UpdateClassCommand
    {
      Id = entity.Id,
      CourseId = Guid.NewGuid(),
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3),
      MaxStudents = 99
    };

    // Act
    entity.ApplyUpdate(command, TestClock.Now);

    // Assert
    Assert.Multiple(() =>
    {
      Assert.That(entity.CourseId, Is.EqualTo(originalCourseId), "a class never moves to another course");
      Assert.That(entity.RegistrationDeadline, Is.EqualTo(command.RegistrationDeadline));
      Assert.That(entity.CourseStartDate, Is.EqualTo(command.CourseStartDate));
      Assert.That(entity.CourseEndDate, Is.EqualTo(command.CourseEndDate));
      Assert.That(entity.MaxStudents, Is.EqualTo(command.MaxStudents));
      Assert.That(entity.UpdatedAt, Is.EqualTo(TestClock.Now));
    });
  }
}
