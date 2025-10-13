using Contracts.Enrollments.Commands;

using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

namespace Test.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

[TestFixture]
public class UnenrollStudentFromClassMapperTests
{
  [Test]
  public void MapToUnenrollStudentFromClassCommand_MapsAllFields()
  {
    var req = new DeleteEnrollmentCommand
    {
      CourseId = "course-1",
      ClassId = "class-1",
      StudentId = "student-1"
    };

    var cmd = req.MapToUnenrollStudentFromClassCommand();

    Assert.That(cmd.CourseId, Is.EqualTo("course-1"));
    Assert.That(cmd.ClassId, Is.EqualTo("class-1"));
    Assert.That(cmd.StudentId, Is.EqualTo("student-1"));
  }
}
