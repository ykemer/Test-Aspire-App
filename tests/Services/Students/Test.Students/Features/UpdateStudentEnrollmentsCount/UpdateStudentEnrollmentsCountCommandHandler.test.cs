using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Students.Database;
using Service.Students.Database.Entities;
using Service.Students.Features.UpdateStudentEnrollmentsCount;

namespace Test.Students.Application.Features.UpdateStudentEnrollmentsCount;

public class UpdateStudentEnrollmentsCountCommandHandlerTest
{
  private UpdateStudentEnrollmentsCountCommandHandler _commandHandler;
  private ApplicationDbContext _dbContext;
  private ILogger<UpdateStudentEnrollmentsCountCommandHandler> _loggerMock;

  [SetUp]
  public void Setup()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase("TestDatabase")
      .Options;
    _dbContext = new ApplicationDbContext(options);
    _loggerMock = Substitute.For<ILogger<UpdateStudentEnrollmentsCountCommandHandler>>();
    _commandHandler = new UpdateStudentEnrollmentsCountCommandHandler(_loggerMock, _dbContext);

    // Clear the database before each test
    _dbContext.Students.RemoveRange(_dbContext.Students);
    _dbContext.SaveChanges();
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldUpdateEnrollmentsCount_WhenStudentExists()
  {
    // Arrange
    var existingStudent = Builder<Student>.CreateNew().With(x => x.EnrollmentsCount, 0).Build();
    await _dbContext.Students.AddAsync(existingStudent);
    await _dbContext.SaveChangesAsync();

    var command = new UpdateStudentEnrollmentsCountCommand(existingStudent.Id, true);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(existingStudent.EnrollmentsCount, Is.EqualTo(1));
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenStudentDoesNotExist()
  {
    // Arrange
    var command = new UpdateStudentEnrollmentsCountCommand("non-existing-student-id", true);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("student_service.update_student_enrollments_count.student_not_found"));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenEnrollmentsCountBecomesNegative()
  {
    // Arrange
    var existingStudent = Builder<Student>.CreateNew().With(x => x.EnrollmentsCount, 0).Build();
    await _dbContext.Students.AddAsync(existingStudent);
    await _dbContext.SaveChangesAsync();

    var command = new UpdateStudentEnrollmentsCountCommand(existingStudent.Id, false);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code,
      Is.EqualTo("student_service.update_student_enrollments_count.invalid_enrollments_count"));
  }
}
