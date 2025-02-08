using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;

namespace Courses.Application.Handlers;

public class DecreaseNumberOfEnrolledStudentsCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private DecreaseNumberOfEnrolledStudentsCommandHandler _commandHandler;
  private Mock<ILogger<DecreaseNumberOfEnrolledStudentsCommandHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<DecreaseNumberOfEnrolledStudentsCommandHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _commandHandler = new DecreaseNumberOfEnrolledStudentsCommandHandler(_loggerMock.Object, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var command = new DecreaseNumberOfEnrolledStudentsCommand("bad-id");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("course_service.decrease_number_of_enrolled_students.course.not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenNoEnrolledStudents()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(course => course.Id, "course-id")
      .With(course => course.EnrollmentsCount, 0)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new DecreaseNumberOfEnrolledStudentsCommand(course.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("course_service.decrease_number_of_enrolled_students.course.no_enrolled_students"));
  }

  [Test]
  public async Task Handle_ShouldDecreaseEnrollmentsCount_WhenCourseExists()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(course => course.EnrollmentsCount, 5)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new DecreaseNumberOfEnrolledStudentsCommand(course.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(4));
  }
}
