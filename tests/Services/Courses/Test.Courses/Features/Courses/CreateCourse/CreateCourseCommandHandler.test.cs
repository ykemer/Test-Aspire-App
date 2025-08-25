using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Database;
using Service.Courses.Database.Entities;
using Service.Courses.Features.Courses.CreateCourse;

namespace Courses.Application.Features.Courses.CreateCourse;

public class CreateCourseCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private CreateCourseCommandHandler _commandHandler;
  private ILogger<CreateCourseCommandHandler> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<CreateCourseCommandHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _commandHandler = new CreateCourseCommandHandler(_dbContext, _loggerMock);
  }


  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnCourse_WhenCourseIsCreated()
  {
    // Arrange
    var command = new CreateCourseCommand("Test Course", "Test Description");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Name, Is.EqualTo("Test Course"));
  }

  [Test]
  public async Task Handle_ShouldReturnError_WhenCourseAlreadyExists()
  {
    // Arrange
    var existingCourse = Builder<Course>
      .CreateNew()
      .With(course => course.Name, "Existing Course")
      .With(course => course.TotalStudents, 5)
      .Build();

    await _dbContext.Courses.AddAsync(existingCourse);
    await _dbContext.SaveChangesAsync();

    var command = new CreateCourseCommand(existingCourse.Name, "New Description");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.create_course.already_exists"));
  }
}
