using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.UpdateClass;

namespace Courses.Application.Features.Classes.UpdateClass;

[TestFixture]
public class UpdateClassCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private UpdateClassCommandHandler _handler;
  private ILogger<UpdateClassCommandHandler> _loggerMock;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<UpdateClassCommandHandler>>();
    _handler = new UpdateClassCommandHandler(_dbContext, _loggerMock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
  {
    var cmd = Builder<UpdateClassCommand>.CreateNew().Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("course_service.update_class.not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnValidation_WhenNewMaxStudentsBelowCurrentTotal()
  {
    var course = Builder<Course>.CreateNew().Build();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, 10)
      .With(c => c.MaxStudents, 50)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = Builder<UpdateClassCommand>.CreateNew()
      .With(c => c.Id, cls.Id)
      .With(c => c.CourseId, course.Id)
      .With(c => c.MaxStudents, 5) // less than current total 10
      .Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("course_service.update_class.max_students_exceeded"));
  }

  [Test]
  public async Task Handle_ShouldUpdate_WhenValid()
  {
    var course = Builder<Course>.CreateNew().Build();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, 5)
      .With(c => c.MaxStudents, 50)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = Builder<UpdateClassCommand>.CreateNew()
      .With(c => c.Id, cls.Id)
      .With(c => c.CourseId, course.Id)
      .With(c => c.MaxStudents, 40)
      .With(c => c.RegistrationDeadline, DateTime.UtcNow.AddDays(-10))
      .With(c => c.CourseStartDate, DateTime.UtcNow.AddDays(-9))
      .With(c => c.CourseEndDate, DateTime.UtcNow.AddDays(-8))
      .Build();

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    var updated = await _dbContext.Classes.FindAsync(cls.Id);
    Assert.That(updated!.MaxStudents, Is.EqualTo(40));
  }
}
