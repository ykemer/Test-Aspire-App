using Contracts.Students.Events;

using FizzWare.NBuilder;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Students.Database;
using Service.Students.Database.Entities;
using Service.Students.Features.DeleteStudent;

using Test.Students.Application.Setup;

namespace Test.Students.Application.Features.DeleteHandler;

public class DeleteStudentCommandHandlerTest
{
  private DeleteStudentCommandHandler _commandHandler;
  private ApplicationDbContext _dbContext;
  private ILogger<DeleteStudentCommandHandler> _loggerMock;
  private IPublishEndpoint _publishEndpointMock;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<DeleteStudentCommandHandler>>();
    _publishEndpointMock = Substitute.For<IPublishEndpoint>();
    _commandHandler = new DeleteStudentCommandHandler(_dbContext, _loggerMock, _publishEndpointMock);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldDeleteStudent_WhenStudentExists()
  {
    // Arrange
    var existingStudent = Builder<Student>.CreateNew().Build();
    await _dbContext.Students.AddAsync(existingStudent);
    await _dbContext.SaveChangesAsync();

    var command = new DeleteStudentCommand(existingStudent.Id);

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Students.CountAsync(), Is.EqualTo(0));
    await _publishEndpointMock.Received(1).Publish(
      Arg.Any<StudentDeletedEvent>(), Arg.Any<CancellationToken>());
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenStudentDoesNotExist()
  {
    // Arrange
    var command = new DeleteStudentCommand("non-existing-student-id");

    // Act
    var result = await _commandHandler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("student_service.delete_student.student_not_found"));
  }
}
