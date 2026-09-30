using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Classes.CreateClass;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private CreateClassCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new CreateClassCommandHandler(_dbContext, NullLogger<CreateClassCommandHandler>.Instance,
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private static CreateClassCommand Command(Guid id) =>
    new()
    {
      Id = id,
      CourseId = Guid.NewGuid(),
      MaxStudents = 20,
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3)
    };

  [Test]
  public async Task Handle_ShouldCopyClass_WithTimestamps()
  {
    var command = Command(Guid.NewGuid());

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var saved = await _dbContext.Classes.FindAsync(command.Id);
    Assert.Multiple(() =>
    {
      Assert.That(saved, Is.Not.Null);
      Assert.That(saved!.CourseId, Is.EqualTo(command.CourseId));
      Assert.That(saved.MaxStudents, Is.EqualTo(20));
      Assert.That(saved.EnrolledCount, Is.Zero);
      Assert.That(saved.CreatedAt, Is.EqualTo(TestClock.Now));
    });
  }

  [Test]
  public async Task Handle_ShouldSucceedWithoutDuplicating_WhenSameClassArrivesTwice()
  {
    var command = Command(Guid.NewGuid());
    await _handler.Handle(command, CancellationToken.None);

    var secondResult = await _handler.Handle(command, CancellationToken.None);

    Assert.That(secondResult.IsError, Is.False, "a repeated message is not an error");
    Assert.That(_dbContext.Classes.Count(), Is.EqualTo(1));
  }
}
