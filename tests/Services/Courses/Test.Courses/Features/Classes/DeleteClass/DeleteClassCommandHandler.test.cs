using Courses.Application.Setup;

using FizzWare.NBuilder;

using MassTransit;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Classes.DeleteClass;

namespace Courses.Application.Features.Classes.DeleteClass;

[TestFixture]
public class DeleteClassCommandHandlerTests
{
  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<DeleteClassCommandHandler>>();
    _publishMock = Substitute.For<IPublishEndpoint>();
    _handler = new DeleteClassCommandHandler(_dbContext, _loggerMock, _publishMock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private ApplicationDbContext _dbContext;
  private DeleteClassCommandHandler _handler;
  private ILogger<DeleteClassCommandHandler> _loggerMock;
  private IPublishEndpoint _publishMock;

  [Test]
  public async Task Handle_ShouldReturnUnexpected_WhenClassNotFound()
  {
    var cmd = new DeleteClassCommand("bad-id", "course-x");

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.delete_course_class.class_not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenClassHasStudents()
  {
    var course = Builder<Course>.CreateNew().Build();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, 3)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new DeleteClassCommand(cls.Id, course.Id);

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.delete_course_class.class_has_students"));
  }

  [Test]
  public async Task Handle_ShouldDeleteAndPublish_WhenValid()
  {
    var course = Builder<Course>.CreateNew().Build();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.TotalStudents, 0)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var cmd = new DeleteClassCommand(cls.Id, course.Id);

    var result = await _handler.Handle(cmd, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Classes.FindAsync(cls.Id) == null, Is.True);
    await _publishMock.ReceivedWithAnyArgs().Publish(default!);
  }
}
