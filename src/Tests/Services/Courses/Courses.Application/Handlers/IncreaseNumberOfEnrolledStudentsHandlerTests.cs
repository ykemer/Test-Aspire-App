using ErrorOr;

using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Courses.Database;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.IncreaseNumberOfEnrolledStudents;

namespace Courses.Application.Handlers;

public class IncreaseNumberOfEnrolledStudentsHandlerTests
{
  private ApplicationDbContext _dbContext;
  private IncreaseNumberOfEnrolledStudentsHandler _handler;
  private Mock<ILogger<IncreaseNumberOfEnrolledStudentsHandler>> _loggerMock;

  [SetUp]
  public void Setup()
  {
    _loggerMock = new Mock<ILogger<IncreaseNumberOfEnrolledStudentsHandler>>();

    DbContextOptions<ApplicationDbContext>? options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);

    _handler = new IncreaseNumberOfEnrolledStudentsHandler(_loggerMock.Object, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    IncreaseNumberOfEnrolledStudentsCommand? command = new("bad-id");

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

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

    IncreaseNumberOfEnrolledStudentsCommand? command = new(course.Id);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(course.EnrollmentsCount, Is.EqualTo(6));
  }
}
