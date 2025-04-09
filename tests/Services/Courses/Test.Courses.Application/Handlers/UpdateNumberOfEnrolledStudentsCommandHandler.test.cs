using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.UpdateNumberOfEnrolledStudents;

namespace Courses.Application.Handlers;

public class UpdateNumberOfEnrolledStudentsCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private UpdateNumberOfEnrolledStudentsCommandHandler _commandHandler;
  private ILogger<UpdateNumberOfEnrolledStudentsCommandHandler> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<UpdateNumberOfEnrolledStudentsCommandHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _commandHandler = new UpdateNumberOfEnrolledStudentsCommandHandler(_loggerMock, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var command = new UpdateNumberOfEnrolledStudentsCommand("bad-id", true);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("course_service.increase_number_of_enrolled_students.course.not_found"));
  }

  [Test]
  public async Task Handle_ShouldIncreaseEnrollmentsCount_WhenCourseExists()
  {
    // Arrange
    var course = Builder<Course>
      .CreateNew()
      .With(course => course.EnrollmentsCount, 5)
      .Build();
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();

    var command = new UpdateNumberOfEnrolledStudentsCommand(course.Id, true);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(6));
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

    var command = new UpdateNumberOfEnrolledStudentsCommand(course.Id, false);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(4));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenEnrollmentsCountBecomesNegative()
  {
    // Arrange
    var existingCourse = Builder<Course>.CreateNew().With(x => x.EnrollmentsCount, 0).Build();
    await _dbContext.Courses.AddAsync(existingCourse);
    await _dbContext.SaveChangesAsync();

    var command = new UpdateNumberOfEnrolledStudentsCommand(existingCourse.Id, false);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("course_service.update_course_enrollments_count.invalid_enrollments_count"));
  }
}
