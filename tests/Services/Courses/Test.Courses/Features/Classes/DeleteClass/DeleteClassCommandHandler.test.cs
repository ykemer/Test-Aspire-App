using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Classes.DeleteClass;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.DeleteClass;

[TestFixture]
public class DeleteClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private DeleteClassCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new DeleteClassCommandHandler(_dbContext, Substitute.For<ILogger<DeleteClassCommandHandler>>());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task<Class> AddClass(int totalStudents)
  {
    var course = Builder<Course>.CreateNew().Build();
    var courseClass = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, totalStudents)
      .Build();
    _dbContext.Courses.Add(course);
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync();
    return courseClass;
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var command = new DeleteClassCommand(Guid.NewGuid(), Guid.NewGuid());

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.NotFound(command.Id, command.CourseId).Code));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassHasStudents()
  {
    var courseClass = await AddClass(totalStudents: 3);

    var result = await _handler.Handle(new DeleteClassCommand(courseClass.Id, courseClass.CourseId),
      CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.HasEnrolledStudents(courseClass.Id).Code));
    Assert.That(await _dbContext.Classes.FindAsync(courseClass.Id), Is.Not.Null);
  }

  [Test]
  public async Task Handle_ShouldDelete_WhenClassHasNoStudents()
  {
    var courseClass = await AddClass(totalStudents: 0);

    var result = await _handler.Handle(new DeleteClassCommand(courseClass.Id, courseClass.CourseId),
      CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(courseClass.Id), Is.Null);
  }
}
