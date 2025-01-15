using ErrorOr;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.CreateCourse;

namespace Courses.Application.Handlers;

public class CreateCourseHandlerTests
{
  private ApplicationDbContext _dbContext;
  private CreateCourseHandler _handler;
  private Mock<ILogger<CreateCourseHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<CreateCourseHandler>>();

    DbContextOptions<ApplicationDbContext>? options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);

    _handler = new CreateCourseHandler(_dbContext, _loggerMock.Object);
  }


  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnCourse_WhenCourseIsCreated()
  {
    // Arrange
    CreateCourseCommand? command = new("Test Course", "Test Description");

    // Act
    ErrorOr<Course> result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Name, Is.EqualTo("Test Course"));
  }

  [Test]
  public async Task Handle_ShouldReturnError_WhenCourseAlreadyExists()
  {
    // Arrange
    Course? existingCourse = Builder<Course>
      .CreateNew()
      .With(course => course.Name, "Existing Course")
      .With(course => course.EnrollmentsCount, 5)
      .Build();

    await _dbContext.Courses.AddAsync(existingCourse);
    await _dbContext.SaveChangesAsync();

    CreateCourseCommand? command = new(existingCourse.Name, "New Description");

    // Act
    ErrorOr<Course> result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.create_course.already_exists"));
  }
}
