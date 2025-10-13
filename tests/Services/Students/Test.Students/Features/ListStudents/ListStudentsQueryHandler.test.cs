using FizzWare.NBuilder;

using Service.Students.Common.Database;
using Service.Students.Common.Database.Entities;
using Service.Students.Features.ListStudent;

using Test.Students.Application.Setup;

namespace Test.Students.Application.Features.ListStudents;

public class ListStudentsQueryHandlerTest
{
  private ApplicationDbContext _dbContext;
  private ListStudentsQueryHandler _queryHandler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _queryHandler = new ListStudentsQueryHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnPagedListOfStudents()
  {
    // Arrange
    var students = Builder<Student>.CreateListOfSize(10).Build();
    await _dbContext.Students.AddRangeAsync(students);
    await _dbContext.SaveChangesAsync();

    var query = new ListStudentsQuery { PageSize = 5, PageNumber = 1 };

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(5));
    Assert.That(result.Value.TotalCount, Is.EqualTo(10));
  }

  [Test]
  public async Task Handle_ShouldReturnSecondPagedListOfStudents()
  {
    // Arrange
    var students = Builder<Student>.CreateListOfSize(10).Build();
    await _dbContext.Students.AddRangeAsync(students);
    await _dbContext.SaveChangesAsync();

    var query = new ListStudentsQuery { PageSize = 5, PageNumber = 2 };

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(5));
    Assert.That(result.Value.TotalCount, Is.EqualTo(10));
  }
}
