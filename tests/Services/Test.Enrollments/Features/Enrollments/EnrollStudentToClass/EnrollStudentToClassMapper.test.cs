using Contracts.Enrollments.Commands;

using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassMapperTests
{
  [Test]
  public void MapToEnrollStudentToClassCommand_MapsAllFields()
  {
    var req = new CreateEnrollmentCommand
    {
      CourseId = "course-1",
      ClassId = "class-1",
      StudentId = "student-1",
      FirstName = "John",
      LastName = "Doe"
    };

    var cmd = req.MapToEnrollStudentToClassCommand();

    Assert.That(cmd.CourseId, Is.EqualTo("course-1"));
    Assert.That(cmd.ClassId, Is.EqualTo("class-1"));
    Assert.That(cmd.StudentId, Is.EqualTo("student-1"));
    Assert.That(cmd.FirstName, Is.EqualTo("John"));
    Assert.That(cmd.LastName, Is.EqualTo("Doe"));
  }
}
