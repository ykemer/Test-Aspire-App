using FizzWare.NBuilder;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Students.Database;
using Service.Students.Entities;
using Service.Students.Features.CreateStudent;

using Test.Students.Application.Setup;

namespace Test.Students.Application.Handlers;

public class CreateStudentCommandHandlerTest
{
  private ApplicationDbContext _dbContext;
  private Mock<ILogger<CreateStudentCommandHandler>> _loggerMock;
  private CreateStudentCommandHandler _commandHandler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = new Mock<ILogger<CreateStudentCommandHandler>>();
    _commandHandler = new CreateStudentCommandHandler(_dbContext, _loggerMock.Object);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldCreateStudent_WhenStudentDoesNotExist()
  {
    // Arrange
    var command = Builder<CreateStudentCommand>.CreateNew().Build();

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Students.CountAsync(), Is.EqualTo(1));
  }

  [Test]
  public async Task Handle_ShouldReturnConflict_WhenStudentWithEmailAlreadyExists()
  {
    // Arrange
    var existingStudent = Builder<Student>.CreateNew()
      .Build();

    await _dbContext.Students.AddAsync(existingStudent);
    await _dbContext.SaveChangesAsync();

    var command = Builder<CreateStudentCommand>.CreateNew()
      .With(c => c.Email = existingStudent.Email)
      .Build();

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("students_service.create_student.already_exists"));
    Assert.That(await _dbContext.Students.CountAsync(), Is.EqualTo(1));
  }
}
