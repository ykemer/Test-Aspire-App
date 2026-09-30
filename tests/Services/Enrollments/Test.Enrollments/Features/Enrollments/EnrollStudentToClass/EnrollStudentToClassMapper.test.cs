using Contracts.Enrollments.Commands;

using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassMapperTests
{
  [Test]
  public void ToEnrollStudentToClassCommand_MapsAllFields()
  {
    var courseId = Guid.NewGuid();
    var classId = Guid.NewGuid();
    var studentId = Guid.NewGuid();
    var idempotencyKey = Guid.NewGuid();
    var req = new CreateEnrollmentCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = studentId,
      FirstName = "John",
      LastName = "Doe",
      IdempotencyKey = idempotencyKey
    };

    var cmd = req.ToEnrollStudentToClassCommand();

    Assert.That(cmd.CourseId, Is.EqualTo(courseId));
    Assert.That(cmd.ClassId, Is.EqualTo(classId));
    Assert.That(cmd.StudentId, Is.EqualTo(studentId));
    Assert.That(cmd.FirstName, Is.EqualTo("John"));
    Assert.That(cmd.LastName, Is.EqualTo("Doe"));
    Assert.That(cmd.IdempotencyKey, Is.EqualTo(idempotencyKey));
  }
}
