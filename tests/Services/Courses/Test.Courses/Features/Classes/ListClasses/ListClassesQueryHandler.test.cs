using Courses.Application.Setup;

using FizzWare.NBuilder;

using Service.Courses.Database;
using Service.Courses.Database.Entities;
using Service.Courses.Features.Classes.ListClasses;

namespace Courses.Application.Features.Classes.ListClasses;

[TestFixture]
public class ListClassesQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private ListClassesQueryHandler _handler;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new ListClassesQueryHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_NotEnrolled_ShouldFilterClosedOrFull()
  {
    var now = DateTime.UtcNow;
    var courseId = Guid.CreateVersion7().ToString();

    var open = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(2))
      .With(c => c.MaxStudents, 20)
      .With(c => c.TotalStudents, 10)
      .Build();

    var closed = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(-1))
      .With(c => c.MaxStudents, 20)
      .With(c => c.TotalStudents, 10)
      .Build();

    var full = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(3))
      .With(c => c.MaxStudents, 10)
      .With(c => c.TotalStudents, 10)
      .Build();

    await _dbContext.Classes.AddRangeAsync(open, closed, full);
    await _dbContext.SaveChangesAsync();

    var query = new ListClassesQuery { CourseId = courseId, PageNumber = 1, PageSize = 10, EnrolledClasses = new List<string>(), ShowAll = false};

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(1)); // only open
    Assert.That(result.Value.Items.Single().Id, Is.EqualTo(open.Id));
  }

  [Test]
  public async Task Handle_Enrolled_ShouldIncludeSpecifiedIds()
  {
    var now = DateTime.UtcNow;
    var courseId = Guid.CreateVersion7().ToString();

    var closedFull = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(-1))
      .With(c => c.MaxStudents, 10)
      .With(c => c.TotalStudents, 10)
      .Build();

    await _dbContext.Classes.AddAsync(closedFull);
    await _dbContext.SaveChangesAsync();

    var query = new ListClassesQuery { CourseId = courseId, PageNumber = 1, PageSize = 10, EnrolledClasses = new List<string>{ closedFull.Id }, ShowAll = false };

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Any(x => x.Id == closedFull.Id), Is.True);
  }

  [Test]
  public async Task Handle_ShowAllTrue_ShouldReturnAllClasses_EvenIfClosedOrFull()
  {
    var now = DateTime.UtcNow;
    var courseId = Guid.CreateVersion7().ToString();

    var open = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(2))
      .With(c => c.MaxStudents, 20)
      .With(c => c.TotalStudents, 10)
      .Build();

    var closed = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(-1))
      .With(c => c.MaxStudents, 20)
      .With(c => c.TotalStudents, 10)
      .Build();

    var full = Builder<Class>.CreateNew()
      .With(c => c.CourseId, courseId)
      .With(c => c.RegistrationDeadline, now.AddDays(3))
      .With(c => c.MaxStudents, 10)
      .With(c => c.TotalStudents, 10)
      .Build();

    await _dbContext.Classes.AddRangeAsync(open, closed, full);
    await _dbContext.SaveChangesAsync();

    var query = new ListClassesQuery { CourseId = courseId, PageNumber = 1, PageSize = 10, EnrolledClasses = new List<string>(), ShowAll = true };

    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Any(x => x.Id == open.Id), Is.True);
    Assert.That(result.Value.Items.Any(x => x.Id == closed.Id), Is.True);
    Assert.That(result.Value.Items.Any(x => x.Id == full.Id), Is.True);
  }
}
