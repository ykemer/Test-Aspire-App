using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.UpdateCourse;

using Test.Courses.Setup;

namespace Test.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseCommandHandlerTests
{
  private FakeTimeProvider _clock = null!;
  private UpdateCourseCommandHandler _handler = null!;
  private ApplicationDbContext _dbContext = null!;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _clock = TestClock.Create();
    _handler = new UpdateCourseCommandHandler(_dbContext, Substitute.For<ILogger<UpdateCourseCommandHandler>>(),
      _clock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task<Course> AddCourse(string name)
  {
    var course = Builder<Course>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = name)
      .With(c => c.UpdatedAt = TestClock.Now)
      .Build();
    _dbContext.Courses.Add(course);
    await _dbContext.SaveChangesAsync();
    return course;
  }

  private static UpdateCourseCommand CommandFor(Guid id, string name) =>
    new() { Id = id, Name = name, Description = "New Description" };

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var command = CommandFor(Guid.NewGuid(), "New Name");

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NotFound(command.Id).Code));
  }

  [Test]
  public async Task Handle_ShouldUpdateCourseAndTimestamp_WhenCourseExists()
  {
    var course = await AddCourse("Old Name");
    _clock.Advance(TimeSpan.FromHours(1));

    var result = await _handler.Handle(CommandFor(course.Id, "New Name"), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var updated = await _dbContext.Courses.AsNoTracking().FirstAsync(x => x.Id == course.Id);
    Assert.Multiple(() =>
    {
      Assert.That(updated.Name, Is.EqualTo("New Name"));
      Assert.That(updated.Description, Is.EqualTo("New Description"));
      Assert.That(updated.UpdatedAt, Is.EqualTo(TestClock.Now.AddHours(1)));
    });
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenNewNameBelongsToAnotherCourse()
  {
    await AddCourse("Taken Name");
    var course = await AddCourse("My Name");

    var result = await _handler.Handle(CommandFor(course.Id, "taken name"), CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NameAlreadyTaken("taken name").Code));
  }

  [Test]
  public async Task Handle_ShouldAllowKeepingTheSameName()
  {
    var course = await AddCourse("Same Name");

    var result = await _handler.Handle(CommandFor(course.Id, "Same Name"), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
  }
}
