using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;
using Moq;

using Service.Students.Database;
using Service.Students.Entities;
using Service.Students.Features.GetStudent;
using Test.Students.Application.Setup;

namespace Test.Students.Application.Handlers;

public class GetStudentHandlerTest
{
  private ApplicationDbContext _dbContext;
  private Mock<ILogger<GetStudentHandler>> _loggerMock;
  private GetStudentHandler _handler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = new Mock<ILogger<GetStudentHandler>>();
    _handler = new GetStudentHandler(_dbContext, _loggerMock.Object);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnStudent_WhenStudentExists()
  {
    // Arrange
    var existingStudent = Builder<Student>.CreateNew().Build();
    await _dbContext.Students.AddAsync(existingStudent);
    await _dbContext.SaveChangesAsync();

    var query = new GetStudentQuery(existingStudent.Id);

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value, Is.EqualTo(existingStudent));
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenStudentDoesNotExist()
  {
    // Arrange
    var query = new GetStudentQuery("non-existing-student-id");

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("students_service.get_student.not_found"));
  }
}
