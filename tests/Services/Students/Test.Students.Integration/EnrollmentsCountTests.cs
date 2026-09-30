using ErrorOr;

using Microsoft.Extensions.Logging.Abstractions;

using Service.Students.Common.Database;
using Service.Students.Features;
using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Test.Students.Integration;

public class EnrollmentsCountTests : PostgresTestBase
{
  private Task<ErrorOr<Updated>> Change(UpdateStudentEnrollmentsCountCommand command) =>
    ChangeWith(DbContext, command);

  private async Task<ErrorOr<Updated>> ChangeWith(ApplicationDbContext dbContext,
    UpdateStudentEnrollmentsCountCommand command) =>
    await new UpdateStudentEnrollmentsCountCommandHandler(NullLogger<UpdateStudentEnrollmentsCountCommandHandler>.Instance,
      dbContext, Clock).Handle(command, CancellationToken.None);

  private async Task<ErrorOr<Updated>> ChangeInParallel(UpdateStudentEnrollmentsCountCommand command)
  {
    await using var dbContext = PostgresDatabase.CreateDbContext();
    return await ChangeWith(dbContext, command);
  }

  private static UpdateStudentEnrollmentsCountCommand Add(Guid studentId) =>
    new(Guid.NewGuid(), studentId, EnrollmentChange.AddEnrollment);

  private static UpdateStudentEnrollmentsCountCommand Remove(Guid studentId) =>
    new(Guid.NewGuid(), studentId, EnrollmentChange.RemoveEnrollment);

  [Test]
  public async Task Add_IncreasesCount()
  {
    var student = await AddStudent(enrollmentsCount: 2);

    var result = await Change(Add(student.Id));

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.EqualTo(3));
  }

  [Test]
  public async Task Remove_DecreasesCount()
  {
    var student = await AddStudent(enrollmentsCount: 2);

    var result = await Change(Remove(student.Id));

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.EqualTo(1));
  }

  [Test]
  public async Task Remove_WhenZero_FailsAndNeverGoesNegative()
  {
    var student = await AddStudent(enrollmentsCount: 0);

    var result = await Change(Remove(student.Id));

    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.HasNoEnrollmentsToRemove(student.Id).Code));
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.Zero);
  }

  [Test]
  public async Task UnknownStudent_ReturnsNotFound()
  {
    var command = Add(Guid.NewGuid());

    var result = await Change(command);

    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.NotFound(command.StudentId).Code));
  }

  [Test]
  public async Task SameMessageDeliveredTwice_IsCountedOnce()
  {
    var student = await AddStudent();
    var command = Add(student.Id);

    var first = await Change(command);
    var second = await ChangeInParallel(command);

    Assert.That(first.IsError, Is.False);
    Assert.That(second.IsError, Is.False, "a duplicate is not an error, it is simply ignored");
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.EqualTo(1));
  }

  [Test]
  public async Task FailedMessage_CanBeRetried_BecauseInboxIsRolledBackToo()
  {
    var student = await AddStudent(enrollmentsCount: 0);
    var command = Remove(student.Id);

    await Change(command);
    var inboxRows = DbContext.InboxMessages.Count();

    Assert.That(inboxRows, Is.Zero);
  }

  [Test]
  public async Task ManyParallelMessages_AreAllCounted()
  {
    var student = await AddStudent();

    var results = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => ChangeInParallel(Add(student.Id))));

    Assert.That(results.Select(result => result.IsError), Is.All.False);
    Assert.That(await ReadEnrollmentsCount(student.Id), Is.EqualTo(20));
  }
}
