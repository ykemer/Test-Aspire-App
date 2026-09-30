using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Students.Common.Database;
using Service.Students.Common.Database.Entities;
using Service.Students.Features;
using Service.Students.Features.GetStudent;

using Test.Students.Setup;

namespace Test.Students.Features.GetStudent;

public class GetStudentQueryHandlerTest
{
  private ApplicationDbContext _dbContext;
  private ILogger<GetStudentQueryHandler> _loggerMock;
  private GetStudentQueryHandler _queryHandler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<GetStudentQueryHandler>>();
    _queryHandler = new GetStudentQueryHandler(_dbContext, _loggerMock);
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
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.Multiple(() =>
    {
      Assert.That(result.Value.Id, Is.EqualTo(existingStudent.Id));
      Assert.That(result.Value.Email, Is.EqualTo(existingStudent.Email));
      Assert.That(result.Value.FirstName, Is.EqualTo(existingStudent.FirstName));
      Assert.That(result.Value.LastName, Is.EqualTo(existingStudent.LastName));
    });
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenStudentDoesNotExist()
  {
    // Arrange
    var query = new GetStudentQuery(Guid.NewGuid());

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo(StudentErrors.NotFound(query.StudentId).Code));
  }
}
