using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Classes;
using Service.Enrollments.Features.Classes.ClassDeleted;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.ClassDeleted;

[TestFixture]
public class DeleteClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private DeleteClassByClassIdCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new DeleteClassByClassIdCommandHandler(_dbContext,
      NullLogger<DeleteClassByClassIdCommandHandler>.Instance);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldSucceed_WhenClassIsAlreadyGone()
  {
    var result = await _handler.Handle(new DeleteClassByClassIdCommand(Guid.NewGuid(), Guid.NewGuid()),
      CancellationToken.None);

    Assert.That(result.IsError, Is.False, "deleting something that is already gone is fine");
  }

  [Test]
  public async Task Handle_ShouldRefuse_WhenClassStillHasEnrollments()
  {
    var courseClass = TestData.OpenClass();
    _dbContext.Classes.Add(courseClass);
    _dbContext.Enrollments.Add(TestData.EnrollmentIn(courseClass));
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteClassByClassIdCommand(courseClass.CourseId, courseClass.Id),
      CancellationToken.None);

    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.HasEnrollments(courseClass.Id).Code));
    Assert.That(await _dbContext.Classes.FindAsync(courseClass.Id), Is.Not.Null);
  }

  [Test]
  public async Task Handle_ShouldDeleteClass_WhenNobodyIsEnrolled()
  {
    var courseClass = TestData.OpenClass();
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteClassByClassIdCommand(courseClass.CourseId, courseClass.Id),
      CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(courseClass.Id), Is.Null);
  }
}
