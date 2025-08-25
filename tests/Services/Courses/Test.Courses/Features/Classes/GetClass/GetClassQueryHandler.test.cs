using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Database;
using Service.Courses.Database.Entities;
using Service.Courses.Features.Classes.GetClass;

namespace Courses.Application.Features.Classes.GetClass;

[TestFixture]
public class GetClassQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetClassQueryHandler _handler;
  private ILogger<GetClassQueryHandler> _loggerMock;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _loggerMock = Substitute.For<ILogger<GetClassQueryHandler>>();
    _handler = new GetClassQueryHandler(_loggerMock, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_NotEnrolled_ShouldReturnWhenOpenAndHasCapacity()
  {
    var now = DateTime.UtcNow;
    var course = Builder<Course>.CreateNew().Build();
    var cls = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.RegistrationDeadline, now.AddDays(1))
      .With(c => c.MaxStudents, 10)
      .With(c => c.TotalStudents, 5)
      .Build();

    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(cls);
    await _dbContext.SaveChangesAsync();

    var query = new GetClassQuery(cls.Id, course.Id, new List<string>(), false);

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(cls.Id));
  }

  [Test]
  public async Task Handle_NotEnrolled_ShouldReturnNotFound_WhenClosedOrFull()
  {
    var now = DateTime.UtcNow;
    var course = Builder<Course>.CreateNew().Build();
    var closedFull = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.RegistrationDeadline, now.AddDays(-1))
      .With(c => c.MaxStudents, 5)
      .With(c => c.TotalStudents, 5)
      .Build();

    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(closedFull);
    await _dbContext.SaveChangesAsync();

    var query = new GetClassQuery(closedFull.Id, course.Id, new List<string>(), false);

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.get_class.not_found"));
  }

  [Test]
  public async Task Handle_Enrolled_ShouldReturnEvenIfClosedOrFull()
  {
    var now = DateTime.UtcNow;
    var course = Builder<Course>.CreateNew().Build();
    var closedFull = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.RegistrationDeadline, now.AddDays(-1))
      .With(c => c.MaxStudents, 5)
      .With(c => c.TotalStudents, 5)
      .Build();

    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(closedFull);
    await _dbContext.SaveChangesAsync();

    var query = new GetClassQuery(closedFull.Id, course.Id, new List<string>{ closedFull.Id }, false);

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(closedFull.Id));
  }

  [Test]
  public async Task Handle_ShowAllTrue_ShouldReturnClass_EvenIfClosedOrFull()
  {
    var now = DateTime.UtcNow;
    var course = Builder<Course>.CreateNew().Build();
    var closedFull = Builder<Class>.CreateNew()
      .With(c => c.CourseId, course.Id)
      .With(c => c.RegistrationDeadline, now.AddDays(-2))
      .With(c => c.MaxStudents, 5)
      .With(c => c.TotalStudents, 5)
      .Build();

    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddAsync(closedFull);
    await _dbContext.SaveChangesAsync();

    var query = new GetClassQuery(closedFull.Id, course.Id, new List<string>(), true); // ShowAll = true

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(closedFull.Id));
  }
}
