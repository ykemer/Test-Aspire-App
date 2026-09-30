using ErrorOr;

using Library.Dates;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Integration;

public class EnrollStudentTests : PostgresTestBase
{
  private Task<ErrorOr<Created>> Enroll(EnrollStudentToClassCommand command) =>
    EnrollWith(DbContext, command);

  private async Task<ErrorOr<Created>> EnrollWith(ApplicationDbContext dbContext, EnrollStudentToClassCommand command) =>
    await new EnrollStudentToClassCommandHandler(NullLogger<EnrollStudentToClassCommandHandler>.Instance, dbContext,
      Clock).Handle(command, CancellationToken.None);

  private async Task<ErrorOr<Created>> EnrollInParallel(EnrollStudentToClassCommand command)
  {
    await using var dbContext = PostgresDatabase.CreateDbContext();
    return await EnrollWith(dbContext, command);
  }

  private static EnrollStudentToClassCommand Command(Class courseClass, Guid? studentId = null) =>
    new()
    {
      CourseId = courseClass.CourseId,
      ClassId = courseClass.Id,
      StudentId = studentId ?? Guid.NewGuid(),
      FirstName = "Jane",
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
    };

  [Test]
  public async Task Enroll_TakesASeat_AndStoresEnrollmentInUtc()
  {
    var courseClass = await AddOpenClass();

    var result = await Enroll(Command(courseClass));

    Assert.That(result.IsError, Is.False);
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
    var enrollment = await DbContext.Enrollments.AsNoTracking().SingleAsync();
    Assert.That(enrollment.EnrollmentDateTime.AsUtc(), Is.EqualTo(Now));
  }

  [Test]
  public async Task Enroll_UnknownClass_ReturnsClassNotFound()
  {
    var command = Command(new Class { CourseId = Guid.NewGuid() });

    var result = await Enroll(command);

    Assert.That(result.FirstError.Code, Is.EqualTo(EnrollmentErrors.ClassNotFound(command.ClassId, command.CourseId).Code));
  }

  [Test]
  public async Task Enroll_WithWrongCourseId_ReturnsClassNotFound()
  {
    var courseClass = await AddOpenClass();

    var result = await Enroll(Command(courseClass) with { CourseId = Guid.NewGuid() });

    Assert.That(result.FirstError.Code, Does.EndWith(".class_not_found"));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((0, 0)));
  }

  [Test]
  public async Task Enroll_AfterDeadline_ReturnsRegistrationClosed()
  {
    var courseClass = await AddOpenClass();
    Clock.Advance(TimeSpan.FromDays(1.5));

    var result = await Enroll(Command(courseClass));

    Assert.That(result.FirstError.Code, Is.EqualTo(EnrollmentErrors.RegistrationClosed(courseClass.Id).Code));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((0, 0)));
  }

  [Test]
  public async Task Enroll_InFullClass_ReturnsClassFull()
  {
    var courseClass = await AddOpenClass(maxStudents: 1);
    await Enroll(Command(courseClass));

    var result = await Enroll(Command(courseClass));

    Assert.That(result.FirstError.Code, Is.EqualTo(EnrollmentErrors.ClassFull(courseClass.Id).Code));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Enroll_SameStudentTwice_ReturnsAlreadyEnrolled()
  {
    var courseClass = await AddOpenClass();
    var studentId = Guid.NewGuid();
    await Enroll(Command(courseClass, studentId));

    var result = await Enroll(Command(courseClass, studentId));

    Assert.That(result.FirstError.Code, Is.EqualTo(EnrollmentErrors.AlreadyEnrolled(studentId, courseClass.Id).Code));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Enroll_SameRequestTwice_EnrollsOnce_AndBothSucceed()
  {
    var courseClass = await AddOpenClass();
    var command = Command(courseClass);

    var first = await Enroll(command);
    var second = await EnrollInParallel(command);

    Assert.That(first.IsError, Is.False);
    Assert.That(second.IsError, Is.False, "a repeated request is not an error");
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Enroll_SameRequestInParallel_EnrollsOnce_AndAllSucceed()
  {
    var courseClass = await AddOpenClass();
    var command = Command(courseClass);

    var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => EnrollInParallel(command)));

    Assert.That(results.Select(result => result.IsError), Is.All.False);
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task Enroll_SameStudentInParallelWithDifferentKeys_EnrollsOnce()
  {
    var courseClass = await AddOpenClass();
    var studentId = Guid.NewGuid();

    var results = await Task.WhenAll(Enumerable.Range(0, 5)
      .Select(_ => EnrollInParallel(Command(courseClass, studentId))));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(1));
    Assert.That(results.Where(result => result.IsError).Select(result => result.FirstError.Code),
      Is.All.EqualTo(EnrollmentErrors.AlreadyEnrolled(studentId, courseClass.Id).Code));
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((1, 1)));
  }

  [Test]
  public async Task ManyStudentsRacingForFewSeats_NeverOverbook()
  {
    const int seats = 3;
    const int students = 20;
    var courseClass = await AddOpenClass(maxStudents: seats);

    var results = await Task.WhenAll(Enumerable.Range(0, students)
      .Select(_ => EnrollInParallel(Command(courseClass))));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(seats));
    Assert.That(results.Where(result => result.IsError).Select(result => result.FirstError.Code),
      Is.All.EqualTo(EnrollmentErrors.ClassFull(courseClass.Id).Code),
      "losers must be told the class is full, not get a random retry error");
    Assert.That(await ReadSeats(courseClass.Id), Is.EqualTo((seats, seats)));
  }
}
