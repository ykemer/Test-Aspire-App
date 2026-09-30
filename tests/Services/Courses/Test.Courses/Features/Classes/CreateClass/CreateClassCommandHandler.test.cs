using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.CreateClass;
using Service.Courses.Features.Courses;

using Test.Courses.Setup;

namespace Test.Courses.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private CreateClassCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new CreateClassCommandHandler(_dbContext, Substitute.For<ILogger<CreateClassCommandHandler>>(),
      TestClock.Create());
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private static CreateClassCommand CommandFor(Guid courseId) =>
    new()
    {
      CourseId = courseId,
      RegistrationDeadline = TestClock.Now.AddDays(1),
      CourseStartDate = TestClock.Now.AddDays(2),
      CourseEndDate = TestClock.Now.AddDays(3),
      MaxStudents = 10
    };

  [Test]
  public async Task Handle_ShouldCreateClass_WhenCourseExists()
  {
    var course = Builder<Course>.CreateNew().Build();
    _dbContext.Courses.Add(course);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(CommandFor(course.Id), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var created = await _dbContext.Classes.FindAsync(result.Value.Id);
    Assert.Multiple(() =>
    {
      Assert.That(created, Is.Not.Null);
      Assert.That(created!.CourseId, Is.EqualTo(course.Id));
      Assert.That(created.MaxStudents, Is.EqualTo(10));
      Assert.That(created.TotalStudents, Is.Zero);
      Assert.That(created.CreatedAt, Is.EqualTo(TestClock.Now));
    });
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var missingCourseId = Guid.NewGuid();

    var result = await _handler.Handle(CommandFor(missingCourseId), CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NotFound(missingCourseId).Code));
    Assert.That(_dbContext.Classes.Count(), Is.Zero);
  }
}
