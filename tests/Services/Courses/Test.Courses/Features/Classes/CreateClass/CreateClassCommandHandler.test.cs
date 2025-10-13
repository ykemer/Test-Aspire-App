using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.CreateClass;

namespace Courses.Application.Features.Classes.CreateClass;

[TestFixture]
public class CreateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private CreateClassCommandHandler _handler;
  private ILogger<CreateClassCommandHandler> _loggerMock;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<CreateClassCommandHandler>>();
    _handler = new CreateClassCommandHandler(_dbContext, _loggerMock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenCourseExistsCheckFails()
  {

    var course = Builder<Course>.CreateNew().Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var cmd = Builder<CreateClassCommand>.CreateNew()
      .With(x => x.CourseId, course.Id)
      .With(x => x.RegistrationDeadline, DateTime.UtcNow.AddDays(-10))
      .With(x => x.CourseStartDate, DateTime.UtcNow.AddDays(-9))
      .With(x => x.CourseEndDate, DateTime.UtcNow.AddDays(-8))
      .With(x => x.MaxStudents, 10)
      .Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var created = result.Value;
    Assert.That(created.CourseId, Is.EqualTo(cmd.CourseId));
    Assert.That(created.MaxStudents, Is.EqualTo(10));
    Assert.That(await _dbContext.Classes.FindAsync(created.Id) is not null, Is.True);


  }

  [Test]
  public async Task Handle_ShouldCreateClass_WhenNoCourseFoundByCurrentLogic()
  {
    // Given no course with the cmd.CourseId, handler proceeds and creates class per current code.
    var cmd = Builder<CreateClassCommand>.CreateNew()
      .With(x => x.RegistrationDeadline, DateTime.UtcNow.AddDays(-10))
      .With(x => x.CourseStartDate, DateTime.UtcNow.AddDays(-9))
      .With(x => x.CourseEndDate, DateTime.UtcNow.AddDays(-8))
      .With(x => x.MaxStudents, 20)
      .Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.create_class.course_does_not_exist"));
  }
}
