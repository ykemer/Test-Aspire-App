using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application.Handlers;

public class UpdateCourseHandlerTests
{
  private ApplicationDbContext _dbContext;
  private UpdateCourseHandler _handler;
  private Mock<ILogger<UpdateCourseHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = new Mock<ILogger<UpdateCourseHandler>>();
    _handler = new UpdateCourseHandler(_dbContext, _loggerMock.Object);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task AddCourseToDatabase(Course course)
  {
    _dbContext.Courses.RemoveRange(_dbContext.Courses); // Clear existing data
    await _dbContext.Courses.AddAsync(course);
    await _dbContext.SaveChangesAsync();
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .Build();

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("course_service.update_course.course.not_found"));
  }

  [Test]
  public async Task Handle_ShouldUpdateCourse_WhenCourseExists()
  {
    // Arrange
    var existingCourse = Builder<Course>.CreateNew()
      .With(c => c.Name = "Old Name")
      .With(c => c.Description = "Old Description")
      .Build();
    await AddCourseToDatabase(existingCourse);

    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id, existingCourse.Id)
      .With(c => c.Name = "New Name")
      .With(c => c.Description = "New Description")
      .Build();

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    var updatedCourse = await _dbContext.Courses.FindAsync(existingCourse.Id);
    Assert.That(updatedCourse.Name, Is.EqualTo("New Name"));
    Assert.That(updatedCourse.Description, Is.EqualTo("New Description"));
  }
}
