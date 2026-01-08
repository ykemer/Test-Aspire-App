using Contracts.Courses.Events;

using Courses.Application.Setup;

using FizzWare.NBuilder;

using MassTransit;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses.DeleteCourse;

namespace Courses.Application.Features.Courses.DeleteCourse;

public class DeleteCourseCommandHandlerTests
{
  private DeleteCourseCommandHandler _commandHandler;
  private ApplicationDbContext _dbContext;
  private ILogger<DeleteCourseCommandHandler> _loggerMock;
  private IPublishEndpoint _messageBusClientMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<DeleteCourseCommandHandler>>();
    _messageBusClientMock = Substitute.For<IPublishEndpoint>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _commandHandler = new DeleteCourseCommandHandler(_dbContext, _loggerMock, _messageBusClientMock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var command = new DeleteCourseCommand("bad-id");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.delete_course.course_not_found"));
  }

  [Test]
  public async Task Handle_ShouldDeleteCourse_WhenCourseExists()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.TotalStudents = 0)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new DeleteCourseCommand(course.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Courses.FindAsync(course.Id), Is.Null);
    await _messageBusClientMock.Received(1).Publish(
      Arg.Is<CourseDeletedEvent>(e => e.CourseId == course.Id), CancellationToken.None);
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenCourseHasStudentsEnrolled()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.TotalStudents = 5)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new DeleteCourseCommand(course.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.delete_course.course_has_students"));
    Assert.That(await _dbContext.Courses.FindAsync(course.Id), Is.Not.Null);
  }
}
