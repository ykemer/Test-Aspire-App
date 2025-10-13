using Courses.Application.Setup;

using FizzWare.NBuilder;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Service.Courses.Common.Database;
using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses.GetCourse;

namespace Courses.Application.Features.Courses.GetCourse;

public class GetCourseQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetCourseQueryHandler _queryHandler;
  private ILogger<GetCourseQueryHandler> _loggerMock;


  [SetUp]
  public void Setup()
  {
    _loggerMock = Substitute.For<ILogger<GetCourseQueryHandler>>();
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _queryHandler = new GetCourseQueryHandler(_loggerMock, _dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnCourse_WhenCourseExists()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Test Course")
      .Build();
    await _dbContext.Courses.AddAsync(course);
    // Add an open class with capacity so course is visible
    await _dbContext.Classes.AddAsync(new Class
    {
      CourseId = course.Id,
      RegistrationDeadline = now.AddDays(2),
      MaxStudents = 20,
      TotalStudents = 5
    });
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseQuery(course.Id, new List<string>(), false);

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(course.Id));
    Assert.That(result.Value.Name, Is.EqualTo("Test Course"));
  }

  [Test]
  public async Task Handle_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    // Arrange
    var query = new GetCourseQuery("bad-id", new  List<string>(), false);

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.get_course.not_found"));
  }

  [Test]
  public async Task Handle_NotEnrolled_OnlyClosedOrFullClasses_ShouldReturnNotFound()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Closed Or Full Course")
      .Build();

    await _dbContext.Courses.AddAsync(course);
    // Add classes: one with past deadline (closed), one full
    await _dbContext.Classes.AddRangeAsync(
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-1), MaxStudents = 10, TotalStudents = 5 },
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(5), MaxStudents = 10, TotalStudents = 10 }
    );
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseQuery(course.Id, new List<string>(), false); // Not enrolled

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.True);
    Assert.That(result.FirstError.Code, Is.EqualTo("courses_service.get_course.not_found"));
  }

  [Test]
  public async Task Handle_NotEnrolled_HasOpenWithCapacityClass_ShouldReturnCourse()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Open Course")
      .Build();

    await _dbContext.Courses.AddAsync(course);
    await _dbContext.Classes.AddRangeAsync(
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(1), MaxStudents = 10, TotalStudents = 5 }, // open & has capacity
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-1), MaxStudents = 10, TotalStudents = 10 } // closed & full
    );
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseQuery(course.Id, new List<string>(), false); // Not enrolled

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(course.Id));
  }

  [Test]
  public async Task Handle_Enrolled_EvenIfClosedOrFull_ShouldReturnCourse()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Enrolled Course")
      .Build();

    await _dbContext.Courses.AddAsync(course);
    var closedFullClass = new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-1), MaxStudents = 10, TotalStudents = 10 };
    await _dbContext.Classes.AddAsync(closedFullClass);
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseQuery(course.Id, new List<string> { closedFullClass.Id}, false); // Enrolled in that class

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(course.Id));
  }

  [Test]
  public async Task Handle_ShowAllTrue_ShouldReturnCourse_EvenIfNoOpenOrEnrolledClasses()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>
      .CreateNew()
      .With(c => c.Name = "Show All Course")
      .Build();

    await _dbContext.Courses.AddAsync(course);
    // Add only closed/full classes
    await _dbContext.Classes.AddRangeAsync(
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-5), MaxStudents = 10, TotalStudents = 10 },
      new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-2), MaxStudents = 10, TotalStudents = 5 }
    );
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseQuery(course.Id, new List<string>(), true); // ShowAll = true

    // Act
    var result = await _queryHandler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Id, Is.EqualTo(course.Id));
    Assert.That(result.Value.Name, Is.EqualTo("Show All Course"));
  }
}
