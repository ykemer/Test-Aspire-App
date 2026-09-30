using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Classes;
using Service.Enrollments.Features.Classes.UpdateClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private UpdateClassCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new UpdateClassCommandHandler(_dbContext, NullLogger<UpdateClassCommandHandler>.Instance,
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private static UpdateClassCommand Command(Guid id, Guid courseId, int maxStudents) =>
    new()
    {
      Id = id,
      CourseId = courseId,
      MaxStudents = maxStudents,
      RegistrationDeadline = TestClock.Now.AddDays(5),
      CourseStartDate = TestClock.Now.AddDays(6),
      CourseEndDate = TestClock.Now.AddDays(7)
    };

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassWasNotCopiedYet()
  {
    var command = Command(Guid.NewGuid(), Guid.NewGuid(), 10);

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.NotFound(command.Id).Code));
  }

  [Test]
  public async Task Handle_ShouldUpdateFieldsAndTimestamp_ButKeepEnrolledCount()
  {
    var courseClass = TestData.OpenClass(enrolledCount: 3);
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(Command(courseClass.Id, courseClass.CourseId, 40), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var updated = await _dbContext.Classes.FindAsync(courseClass.Id);
    Assert.Multiple(() =>
    {
      Assert.That(updated!.MaxStudents, Is.EqualTo(40));
      Assert.That(updated.RegistrationDeadline, Is.EqualTo(TestClock.Now.AddDays(5)));
      Assert.That(updated.EnrolledCount, Is.EqualTo(3), "the count is owned by this service, not copied");
      Assert.That(updated.UpdatedAt, Is.EqualTo(TestClock.Now));
    });
  }
}
