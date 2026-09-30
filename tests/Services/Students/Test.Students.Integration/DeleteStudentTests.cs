using ErrorOr;

using Microsoft.Extensions.Logging.Abstractions;

using Service.Students.Features;
using Service.Students.Features.DeleteStudent;

namespace Test.Students.Integration;

public class DeleteStudentTests : PostgresTestBase
{
  private Task<ErrorOr<Deleted>> Delete(Guid studentId) =>
    new DeleteStudentCommandHandler(DbContext, NullLogger<DeleteStudentCommandHandler>.Instance)
      .Handle(new DeleteStudentCommand(studentId), CancellationToken.None).AsTask();

  [Test]
  public async Task Delete_StudentWithoutEnrollments_Succeeds()
  {
    var student = await AddStudent(enrollmentsCount: 0);

    var result = await Delete(student.Id);

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.Null, "the student is gone");
  }

  [Test]
  public async Task Delete_StudentWithEnrollments_IsRefused()
  {
    var student = await AddStudent(enrollmentsCount: 1);

    var result = await Delete(student.Id);

    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.HasEnrollments(student.Id).Code));
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.EqualTo(1));
  }

  [Test]
  public async Task Delete_UnknownStudent_ReturnsNotFound()
  {
    var studentId = Guid.NewGuid();

    var result = await Delete(studentId);

    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.NotFound(studentId).Code));
  }
}
