using Contracts.Courses.Events;

using ErrorOr;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.AsyncDataServices;
using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.DeleteCourse;

namespace Courses.Application.Handlers;

public class DeleteCourseHandlerTests
{
  private ApplicationDbContext _dbContext;
  private DeleteCourseHandler _handler;
  private Mock<ILogger<DeleteCourseHandler>> _loggerMock;
  private Mock<IMessageBusClient> _messageBusClientMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<DeleteCourseHandler>>();
    _messageBusClientMock = new Mock<IMessageBusClient>();

    DbContextOptions<ApplicationDbContext>? options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);
    _handler = new DeleteCourseHandler(_dbContext, _loggerMock.Object, _messageBusClientMock.Object);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    DeleteCourseCommand? command = new("bad-id");

    // Act
    ErrorOr<Deleted> result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.delete_course.course_not_found"));
  }

  [Test]
  public async Task Handle_ShouldDeleteCourse_WhenCourseExists()
  {
    // Arrange
    Course? course = Builder<Course>
      .CreateNew()
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    DeleteCourseCommand? command = new(course.Id);

    // Act
    ErrorOr<Deleted> result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Courses.FindAsync(course.Id), Is.Null);
    _messageBusClientMock.Verify(
      m => m.PublishCourseDeletedMessage(It.Is<CourseDeletedEvent>(e => e.CourseId == course.Id)), Times.Once);
  }
}
