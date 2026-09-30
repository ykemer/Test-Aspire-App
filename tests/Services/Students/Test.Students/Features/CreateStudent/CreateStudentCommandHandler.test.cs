using Microsoft.Extensions.Logging.Abstractions;

using Service.Students.Common.Database;
using Service.Students.Features;
using Service.Students.Features.CreateStudent;

using Test.Students.Setup;

namespace Test.Students.Features.CreateStudent;

[TestFixture]
public class CreateStudentCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private CreateStudentCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new CreateStudentCommandHandler(_dbContext, NullLogger<CreateStudentCommandHandler>.Instance,
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private static CreateStudentCommand Command(string email = "jane.doe@example.com") =>
    new()
    {
      Id = Guid.NewGuid(),
      FirstName = "Jane",
      LastName = "Doe",
      Email = email,
      DateOfBirth = new DateTime(2000, 5, 17, 10, 30, 0, DateTimeKind.Utc)
    };

  [Test]
  public async Task Handle_ShouldCreateStudent_WithTimestampsAndDateOnlyBirthday()
  {
    var command = Command();

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var student = await _dbContext.Students.FindAsync(command.Id);
    Assert.Multiple(() =>
    {
      Assert.That(student!.Email, Is.EqualTo(command.Email));
      Assert.That(student.DateOfBirth, Is.EqualTo(new DateTime(2000, 5, 17)), "time of day is dropped");
      Assert.That(student.EnrollmentsCount, Is.Zero);
      Assert.That(student.CreatedAt, Is.EqualTo(TestClock.Now));
    });
  }

  [Test]
  public async Task Handle_ShouldSucceedWithoutDuplicating_WhenSameUserArrivesTwice()
  {
    var command = Command();
    await _handler.Handle(command, CancellationToken.None);

    var second = await _handler.Handle(command, CancellationToken.None);

    Assert.That(second.IsError, Is.False, "a repeated message is not an error");
    Assert.That(_dbContext.Students.Count(), Is.EqualTo(1));
  }

  [Test]
  public async Task Handle_ShouldRefuse_WhenAnotherStudentHasTheSameEmail_IgnoringCase()
  {
    await _handler.Handle(Command("jane.doe@example.com"), CancellationToken.None);

    var result = await _handler.Handle(Command("JANE.DOE@example.com"), CancellationToken.None);

    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.EmailAlreadyTaken("x").Code));
    Assert.That(_dbContext.Students.Count(), Is.EqualTo(1));
  }
}
