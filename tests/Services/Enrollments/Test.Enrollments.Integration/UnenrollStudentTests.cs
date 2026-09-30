using ErrorOr;

using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;
using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

namespace Test.Enrollments.Integration;

public class UnenrollStudentTests : PostgresTestBase
{
  private async Task<ErrorOr<Deleted>> UnenrollWith(ApplicationDbContext dbContext,
    UnenrollStudentFromClassCommand command) =>
    await new UnenrollStudentFromClassCommandHandler(NullLogger<UnenrollStudentFromClassCommandHandler>.Instance,
      dbContext, Clock).Handle(command, CancellationToken.None);

  private async Task<ErrorOr<Deleted>> UnenrollInParallel(UnenrollStudentFromClassCommand command)
  {
    await using var dbContext = PostgresDatabase.CreateDbContext();
    return await UnenrollWith(dbContext, command);
  }

  /// <summary>Enrolls one student through the real handler, so the seat count is correct.</summary>
  private async Task<Guid> EnrollStudent(Class courseClass)
  {
    var studentId = Guid.NewGuid();
    await using var dbContext = PostgresDatabase.CreateDbContext();
    await new EnrollStudentToClassCommandHandler(NullLogger<EnrollStudentToClassCommandHandler>.Instance, dbContext,
      Clock).Handle(new EnrollStudentToClassCommand
    {
      CourseId = courseClass.CourseId,
      ClassId = courseClass.Id,
      StudentId = studentId,
      FirstName = "Jane",
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
    }, CancellationToken.None);
    return studentId;
  }

  private static UnenrollStudentFromClassCommand Command(Class courseClass, Guid studentId) =>
    new()
    {
      CourseId = courseClass.CourseId, ClassId = courseClass.Id, StudentId = studentId, IdempotencyKey = Guid.NewGuid()
    };

  [Test]
  public async Task Unenroll_RemovesStudent_AndFreesSeat()
  {
    var courseClass = await AddOpenClass();
    var studentId = await EnrollStudent(courseClass);

    var result = await UnenrollWith(DbContext, Command(courseClass, studentId));

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((0, 0)));
  }

  [Test]
  public async Task Unenroll_StudentWhoIsNotEnrolled_ReturnsNotEnrolled()
  {
    var courseClass = await AddOpenClass();
    var command = Command(courseClass, Guid.NewGuid());

    var result = await UnenrollWith(DbContext, command);

    Assert.That(result.FirstError.Code,
      Is.EqualTo(EnrollmentErrors.NotEnrolled(command.StudentId, command.ClassId).Code));
  }

  [Test]
  public async Task Unenroll_AfterClassStarted_IsRefused()
  {
    var courseClass = await AddOpenClass();
    var studentId = await EnrollStudent(courseClass);
    Clock.Advance(TimeSpan.FromDays(2));

    var result = await UnenrollWith(DbContext, Command(courseClass, studentId));

    Assert.That(result.FirstError.Code, Is.EqualTo(EnrollmentErrors.ClassAlreadyStarted(courseClass.Id).Code));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Unenroll_SameRequestTwice_FreesOneSeat_AndBothSucceed()
  {
    var courseClass = await AddOpenClass();
    await EnrollStudent(courseClass);
    var studentId = await EnrollStudent(courseClass);
    var command = Command(courseClass, studentId);

    var first = await UnenrollWith(DbContext, command);
    var second = await UnenrollInParallel(command);

    Assert.That(first.IsError, Is.False);
    Assert.That(second.IsError, Is.False, "a repeated request is not an error");
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Unenroll_SameStudentInParallelWithDifferentKeys_FreesOneSeat()
  {
    var courseClass = await AddOpenClass();
    await EnrollStudent(courseClass);
    var studentId = await EnrollStudent(courseClass);

    var results = await Task.WhenAll(Enumerable.Range(0, 5)
      .Select(_ => UnenrollInParallel(Command(courseClass, studentId))));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(1));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)), "the other student keeps their seat");
  }
}
