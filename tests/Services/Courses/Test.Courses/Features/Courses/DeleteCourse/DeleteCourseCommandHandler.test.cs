using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.DeleteCourse;

using Test.Courses.Setup;

namespace Test.Courses.Features.Courses.DeleteCourse;

public class DeleteCourseCommandHandlerTests
{
  private DeleteCourseCommandHandler _handler = null!;
  private ApplicationDbContext _dbContext = null!;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new DeleteCourseCommandHandler(_dbContext, Substitute.For<ILogger<DeleteCourseCommandHandler>>());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var command = new DeleteCourseCommand(Guid.NewGuid());

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NotFound(command.Id).Code));
  }

  [Test]
  public async Task Handle_ShouldDeleteCourseAndItsClasses_WhenNoStudentsEnrolled()
  {
    var course = Builder<Course>.CreateNew().With(c => c.TotalStudents = 0).Build();
    var courseClass = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, 0)
      .Build();
    _dbContext.Courses.Add(course);
    _dbContext.Classes.Add(courseClass);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteCourseCommand(course.Id), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Courses.FindAsync(course.Id), Is.Null);
    Assert.That(await _dbContext.Classes.FindAsync(courseClass.Id), Is.Null);
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenCourseHasStudentsEnrolled()
  {
    var course = Builder<Course>.CreateNew().With(c => c.TotalStudents = 5).Build();
    _dbContext.Courses.Add(course);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteCourseCommand(course.Id), CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.HasEnrolledStudents(course.Id).Code));
    Assert.That(await _dbContext.Courses.FindAsync(course.Id), Is.Not.Null);
  }
}
