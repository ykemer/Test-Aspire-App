using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes;
using Service.Courses.Features.Classes.UpdateClass;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private UpdateClassCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new UpdateClassCommandHandler(_dbContext, Substitute.For<ILogger<UpdateClassCommandHandler>>(),
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task<Class> AddClass(int totalStudents, int maxStudents)
  {
    var course = Builder<Course>.CreateNew().Build();
    var courseClass = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, totalStudents)
      .With(c => c.MaxStudents, maxStudents)
      .Build();
    _dbContext.Courses.Add(course);
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync();
    return courseClass;
  }

  private static UpdateClassCommand CommandFor(Class courseClass, int maxStudents) =>
    new()
    {
      Id = courseClass.Id,
      CourseId = courseClass.CourseId,
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3),
      MaxStudents = maxStudents
    };

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var command = CommandFor(Builder<Class>.CreateNew().Build(), 10);

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.NotFound(command.Id, command.CourseId).Code));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenNewMaxStudentsBelowCurrentTotal()
  {
    var courseClass = await AddClass(totalStudents: 10, maxStudents: 50);

    var result = await _handler.Handle(CommandFor(courseClass, maxStudents: 5), CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.MaxStudentsBelowEnrolledCount(courseClass.Id).Code));
  }

  [Test]
  public async Task Handle_ShouldUpdateFieldsAndTimestamp_WhenValid()
  {
    var courseClass = await AddClass(totalStudents: 5, maxStudents: 50);

    var result = await _handler.Handle(CommandFor(courseClass, maxStudents: 40), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var updated = await _dbContext.Classes.FindAsync(courseClass.Id);
    Assert.That(updated!.MaxStudents, Is.EqualTo(40));
    Assert.That(updated.UpdatedAt, Is.EqualTo(TestClock.Now));
  }
}
