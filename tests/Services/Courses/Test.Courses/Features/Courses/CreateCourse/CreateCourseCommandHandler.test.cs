using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.CreateCourse;

using Test.Courses.Setup;

namespace Test.Courses.Features.Courses.CreateCourse;

public class CreateCourseCommandHandlerTests
{
  private CreateCourseCommandHandler _handler = null!;
  private ApplicationDbContext _dbContext = null!;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new CreateCourseCommandHandler(_dbContext, Substitute.For<ILogger<CreateCourseCommandHandler>>(),
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldCreateCourse_WithTimestamps()
  {
    var command = new CreateCourseCommand("Test Course", "Test Description");

    var result = await _handler.Handle(command, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var saved = await _dbContext.Courses.FindAsync(result.Value.Id);
    Assert.Multiple(() =>
    {
      Assert.That(saved!.Name, Is.EqualTo("Test Course"));
      Assert.That(saved.CreatedAt, Is.EqualTo(TestClock.Now));
      Assert.That(saved.UpdatedAt, Is.EqualTo(TestClock.Now));
    });
  }

  [TestCase("Existing Course")]
  [TestCase("EXISTING course")]
  public async Task Handle_ShouldReturnConflict_WhenNameIsTaken_IgnoringCase(string newName)
  {
    var existingCourse = Builder<Course>.CreateNew().With(course => course.Name, "Existing Course").Build();
    _dbContext.Courses.Add(existingCourse);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new CreateCourseCommand(newName, "New Description"), CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NameAlreadyTaken(newName).Code));
  }
}
