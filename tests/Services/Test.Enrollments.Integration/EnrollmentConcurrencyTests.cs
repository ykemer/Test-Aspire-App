using ErrorOr;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Integration;

[TestFixture]
public class EnrollmentConcurrencyTests
{
  [Test]
  public async Task Handle_ShouldLetExactlyOneWinner_WhenTwoRequestsRaceForTheLastSeat()
  {
    var classId = Guid.NewGuid();
    var courseId = Guid.NewGuid();
    var student1Id = Guid.NewGuid();
    var student2Id = Guid.NewGuid();

    await using (var seedContext = PostgresContainerFixture.CreateDbContext())
    {
      await seedContext.Classes.AddAsync(new Class
      {
        Id = classId,
        CourseId = courseId,
        MaxStudents = 1,
        EnrolledCount = 0,
        RegistrationDeadline = DateTime.UtcNow.AddDays(1),
        CourseStartDate = DateTime.UtcNow.AddDays(2),
        CourseEndDate = DateTime.UtcNow.AddDays(3)
      });
      await seedContext.SaveChangesAsync();
    }

    var results = await Task.WhenAll(
      EnrollAsync(courseId, classId, student1Id, "First"),
      EnrollAsync(courseId, classId, student2Id, "Second"));

    Assert.That(results.Count(r => !r.IsError), Is.EqualTo(1),
      "exactly one of the two concurrent requests should win the last seat");

    var loser = results.Single(r => r.IsError);
    Assert.That(loser.FirstError.Code, Is.AnyOf(
      "enrollment_service.enroll_student_to_course.class_full",
      "enrollment_service.enroll_student_to_course.concurrency_conflict"));

    await using var verifyContext = PostgresContainerFixture.CreateDbContext();
    var finalClass = await verifyContext.Classes.FirstAsync(c => c.Id == classId);
    Assert.That(finalClass.EnrolledCount, Is.EqualTo(1), "the class must not be overbooked");
    Assert.That(await verifyContext.Enrollments.CountAsync(e => e.ClassId == classId), Is.EqualTo(1));
  }

  private static async Task<ErrorOr<Created>> EnrollAsync(Guid courseId, Guid classId, Guid studentId,
    string firstName)
  {
    await using var dbContext = PostgresContainerFixture.CreateDbContext();
    var handler = new EnrollStudentToClassCommandHandler(
      Substitute.For<ILogger<EnrollStudentToClassCommandHandler>>(), dbContext);

    return await handler.Handle(new EnrollStudentToClassCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = studentId,
      FirstName = firstName,
      LastName = "Student",
      IdempotencyKey = Guid.NewGuid()
    }, CancellationToken.None);
  }
}
