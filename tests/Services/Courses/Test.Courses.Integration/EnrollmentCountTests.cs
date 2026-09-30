using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Classes.UpdateNumberOfEnrolledStudents;
using Service.Courses.Features.Courses;

namespace Test.Courses.Integration;

public class EnrollmentCountTests : PostgresTestBase
{
  private UpdateNumberOfEnrolledStudentsCommandHandler CreateHandler(ApplicationDbContext dbContext) =>
    new(NullLogger<UpdateNumberOfEnrolledStudentsCommandHandler>.Instance, dbContext, Clock);

  private async Task<Class> AddClass(int totalStudents, int maxStudents)
  {
    var course = new Course { Name = $"Course {Guid.NewGuid()}", Description = "Description", TotalStudents = totalStudents };
    var courseClass = new Class
    {
      CourseId = course.Id,
      RegistrationDeadline = Clock.GetUtcNow().UtcDateTime.AddDays(1),
      CourseStartDate = Clock.GetUtcNow().UtcDateTime.AddDays(2),
      CourseEndDate = Clock.GetUtcNow().UtcDateTime.AddDays(3),
      MaxStudents = maxStudents,
      TotalStudents = totalStudents
    };
    DbContext.Courses.Add(course);
    DbContext.Classes.Add(courseClass);
    await DbContext.SaveChangesAsync();
    DbContext.ChangeTracker.Clear();
    return courseClass;
  }

  private static UpdateNumberOfEnrolledStudentsCommand Command(Class courseClass, EnrollmentChange change) =>
    new(Guid.NewGuid(), courseClass.CourseId, courseClass.Id, change);

  private async Task<(int CourseTotal, int ClassTotal)> ReadTotals(Class courseClass)
  {
    await using var freshContext = PostgresDatabase.CreateDbContext();
    var courseTotal = await freshContext.Courses.Where(c => c.Id == courseClass.CourseId)
      .Select(c => c.TotalStudents).SingleAsync();
    var classTotal = await freshContext.Classes.Where(c => c.Id == courseClass.Id)
      .Select(c => c.TotalStudents).SingleAsync();
    return (courseTotal, classTotal);
  }

  [Test]
  public async Task AddStudent_IncreasesClassAndCourse()
  {
    var courseClass = await AddClass(totalStudents: 3, maxStudents: 10);

    var result = await CreateHandler(DbContext).Handle(Command(courseClass, EnrollmentChange.AddStudent), default);

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((4, 4)));
  }

  [Test]
  public async Task RemoveStudent_DecreasesClassAndCourse()
  {
    var courseClass = await AddClass(totalStudents: 3, maxStudents: 10);

    var result = await CreateHandler(DbContext).Handle(Command(courseClass, EnrollmentChange.RemoveStudent), default);

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((2, 2)));
  }

  [Test]
  public async Task AddStudent_ToFullClass_FailsAndChangesNothing()
  {
    var courseClass = await AddClass(totalStudents: 10, maxStudents: 10);

    var result = await CreateHandler(DbContext).Handle(Command(courseClass, EnrollmentChange.AddStudent), default);

    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.IsFull(courseClass.Id).Code));
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((10, 10)), "course counter must be rolled back too");
  }

  [Test]
  public async Task RemoveStudent_WhenNobodyEnrolled_FailsAndNeverGoesNegative()
  {
    var courseClass = await AddClass(totalStudents: 0, maxStudents: 10);

    var result = await CreateHandler(DbContext).Handle(Command(courseClass, EnrollmentChange.RemoveStudent), default);

    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.HasNoStudentsToRemove(courseClass.CourseId).Code));
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((0, 0)));
  }

  [Test]
  public async Task UnknownCourse_ReturnsCourseNotFound()
  {
    var command = new UpdateNumberOfEnrolledStudentsCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
      EnrollmentChange.AddStudent);

    var result = await CreateHandler(DbContext).Handle(command, default);

    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NotFound(command.CourseId).Code));
  }

  [Test]
  public async Task UnknownClass_ReturnsClassNotFound_AndCourseIsRolledBack()
  {
    var courseClass = await AddClass(totalStudents: 1, maxStudents: 10);
    var command = new UpdateNumberOfEnrolledStudentsCommand(Guid.NewGuid(), courseClass.CourseId, Guid.NewGuid(),
      EnrollmentChange.AddStudent);

    var result = await CreateHandler(DbContext).Handle(command, default);

    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.NotFound(command.ClassId, command.CourseId).Code));
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task SameMessageDeliveredTwice_IsCountedOnce()
  {
    var courseClass = await AddClass(totalStudents: 0, maxStudents: 10);
    var command = Command(courseClass, EnrollmentChange.AddStudent);

    var first = await CreateHandler(DbContext).Handle(command, default);
    await using var secondContext = PostgresDatabase.CreateDbContext();
    var second = await CreateHandler(secondContext).Handle(command, default);

    Assert.That(first.IsError, Is.False);
    Assert.That(second.IsError, Is.False, "a duplicate is not an error, it is simply ignored");
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task FailedMessage_CanBeRetried_BecauseInboxIsRolledBackToo()
  {
    var courseClass = await AddClass(totalStudents: 0, maxStudents: 10);
    var command = Command(courseClass, EnrollmentChange.RemoveStudent);

    var failed = await CreateHandler(DbContext).Handle(command, default);

    Assert.That(failed.IsError, Is.True);
    Assert.That(await DbContext.InboxMessages.CountAsync(), Is.Zero);
  }

  [Test]
  public async Task ManyParallelEnrollments_NeverExceedMaxStudents()
  {
    const int maxStudents = 5;
    const int attempts = 20;
    var courseClass = await AddClass(totalStudents: 0, maxStudents: maxStudents);

    var results = await Task.WhenAll(Enumerable.Range(0, attempts).Select(async _ =>
    {
      await using var context = PostgresDatabase.CreateDbContext();
      return await CreateHandler(context).Handle(Command(courseClass, EnrollmentChange.AddStudent), default);
    }));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(maxStudents));
    Assert.That(await ReadTotals(courseClass), Is.EqualTo((maxStudents, maxStudents)));
  }
}
