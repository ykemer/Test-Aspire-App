using Contracts.Enrollments.Requests;

using Platform.Features.Enrollments;

using Test.Platform.Setup;

namespace Test.Platform.Features.Enrollments;

[TestFixture]
public class EnrollmentStudentTests
{
  [Test]
  public void Student_AlwaysActsForThemselves_EvenIfAnotherIdIsSent()
  {
    var studentId = Guid.NewGuid();
    var request = new ChangeCourseEnrollmentRequest { StudentId = Guid.NewGuid() };

    var result = EnrollmentStudent.Resolve(TestUsers.Student(studentId), request);

    Assert.That(result.Value, Is.EqualTo(studentId), "a student must not be able to enroll someone else");
  }

  [Test]
  public void Administrator_ActsForTheGivenStudent()
  {
    var request = new ChangeCourseEnrollmentRequest { StudentId = Guid.NewGuid() };

    var result = EnrollmentStudent.Resolve(TestUsers.Administrator(), request);

    Assert.That(result.Value, Is.EqualTo(request.StudentId));
  }

  [Test]
  public void Administrator_WithoutStudentId_GetsAValidationError()
  {
    var result = EnrollmentStudent.Resolve(TestUsers.Administrator(), new ChangeCourseEnrollmentRequest());

    Assert.That(result.FirstError, Is.EqualTo(EnrollmentStudent.MissingStudentId));
  }
}
