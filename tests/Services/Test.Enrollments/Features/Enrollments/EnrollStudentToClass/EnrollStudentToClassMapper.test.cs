using Contracts.Enrollments.Commands;

using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassMapperTests
{
  [Test]
  public void MapToEnrollStudentToClassCommand_MapsAllFields()
  {
    var courseId = Guid.NewGuid();
    var classId = Guid.NewGuid();
    var studentId = Guid.NewGuid();
    var req = new CreateEnrollmentCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe"
    };

    var cmd = req.MapToEnrollStudentToClassCommand();

    Assert.That(cmd.CourseId, Is.EqualTo(courseId));
    Assert.That(cmd.ClassId, Is.EqualTo(classId));
    Assert.That(cmd.StudentId, Is.EqualTo(studentId));
    Assert.That(cmd.FirstName, Is.EqualTo("John"));
    Assert.That(cmd.LastName, Is.EqualTo("Doe"));
  }
}
