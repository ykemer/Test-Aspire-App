using ErrorOr;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.DecreaseNumberOfEnrolledStudents;

namespace Courses.Application.Handlers;

public class DecreaseNumberOfEnrolledStudentsHandlerTests
{
  private ApplicationDbContext _dbContext;
  private DecreaseNumberOfEnrolledStudentsHandler _handler;
  private Mock<ILogger<DecreaseNumberOfEnrolledStudentsHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<DecreaseNumberOfEnrolledStudentsHandler>>();

    DbContextOptions<ApplicationDbContext>? options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);

    _handler = new DecreaseNumberOfEnrolledStudentsHandler(_loggerMock.Object, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    DecreaseNumberOfEnrolledStudentsCommand? command = new("bad-id");

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

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

    DecreaseNumberOfEnrolledStudentsCommand? command = new(course.Id);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

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

    DecreaseNumberOfEnrolledStudentsCommand? command = new(course.Id);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(4));
  }
}
