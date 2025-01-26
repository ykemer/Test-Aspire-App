using Contracts.Students.Events;

using FizzWare.NBuilder;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Service.Students.Database;
using Service.Students.Entities;
using Service.Students.Features.DeleteStudent;

using Test.Students.Application.Setup;

namespace Test.Students.Application.Handlers;

public class DeleteStudentHandlerTest
{

  private ApplicationDbContext _dbContext;
  private Mock<ILogger<DeleteStudentHandler>> _loggerMock;
  private Mock<IPublishEndpoint> _publishEndpointMock;
  private DeleteStudentHandler _handler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = new Mock<ILogger<DeleteStudentHandler>>();
    _publishEndpointMock = new Mock<IPublishEndpoint>();
    _handler = new DeleteStudentHandler(_dbContext, _loggerMock.Object, _publishEndpointMock.Object);
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
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(await _dbContext.Students.CountAsync(), Is.EqualTo(0));
    _publishEndpointMock.Verify(p => p.Publish(It.IsAny<StudentDeletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenStudentDoesNotExist()
  {
    // Arrange
    var command = new DeleteStudentCommand("non-existing-student-id");

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("student_service.delete_student.student_not_found"));
  }
}
