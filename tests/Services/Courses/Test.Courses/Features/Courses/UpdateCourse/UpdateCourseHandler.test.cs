using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application.Features.Courses.UpdateCourse;

public class UpdateCourseCommandHandlerTests
{
  private ApplicationDbContext _dbContext;
  private UpdateCourseCommandHandler _commandHandler;
  private ILogger<UpdateCourseCommandHandler> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<UpdateCourseCommandHandler>>();
    _commandHandler = new UpdateCourseCommandHandler(_dbContext, _loggerMock);
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
    var result = await _commandHandler.Handle(command, CancellationToken.None);

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
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    var updatedCourse = await _dbContext.Courses.AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == existingCourse.Id, CancellationToken.None);
    Assert.That(updatedCourse?.Name, Is.EqualTo("New Name"));
    Assert.That(updatedCourse.Description, Is.EqualTo("New Description"));
  }
}
