using Contracts.Enrollments.Commands;

using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

namespace Test.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

[TestFixture]
public class UnenrollStudentFromClassMapperTests
{
  [Test]
  public void MapToUnenrollStudentFromClassCommand_MapsAllFields()
  {
    var courseId = Guid.NewGuid();
    var classId = Guid.NewGuid();
    var studentId = Guid.NewGuid();
    var req = new DeleteEnrollmentCommand { CourseId = courseId, ClassId = classId, StudentId = studentId };

    var cmd = req.MapToUnenrollStudentFromClassCommand();

    Assert.That(cmd.CourseId, Is.EqualTo(courseId));
    Assert.That(cmd.ClassId, Is.EqualTo(classId));
    Assert.That(cmd.StudentId, Is.EqualTo(studentId));
  }
}
