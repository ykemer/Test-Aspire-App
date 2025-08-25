using Courses.Application.Setup;

using FizzWare.NBuilder;

using Service.Courses.Database;
using Service.Courses.Database.Entities;
using Service.Courses.Features.Courses.ListCourses;

namespace Courses.Application.Features.Courses.ListCourses;

public class ListCoursesQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private ListCoursesQueryHandler _queryHandler;

  [SetUp]
  public void Setup()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _queryHandler = new ListCoursesQueryHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private async Task AddCoursesToDatabase(params Course[] courses)
  {
    _dbContext.Courses.RemoveRange(_dbContext.Courses);
    await _dbContext.Courses.AddRangeAsync(courses);
    await _dbContext.SaveChangesAsync();
  }

  [Test]
  public async Task Handle_ShouldReturnPagedList_WhenCoursesExist()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var courses = Builder<Course>.CreateListOfSize(3).Build().ToArray();
    await AddCoursesToDatabase(courses);

    // Ensure each course has an open class with capacity so it is visible
    foreach (var c in courses)
    {
      await _dbContext.Classes.AddAsync(new Class
      {
        CourseId = c.Id,
        RegistrationDeadline = now.AddDays(3),
        MaxStudents = 30,
        TotalStudents = 5
      });
    }
    await _dbContext.SaveChangesAsync();

    var request = new ListCoursesRequest{ PageNumber = 1, PageSize = 2 };

    // Act
    var result = await _queryHandler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(2));
    Assert.That(result.Value.TotalCount, Is.EqualTo(3));
  }

  [Test]
  public async Task Handle_ShouldReturnPagedList_WhenRequestingSecondPage()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var courses = Builder<Course>.CreateListOfSize(3).Build().ToArray();
    await AddCoursesToDatabase(courses);

    // Ensure each course has an open class with capacity so it is visible
    foreach (var c in courses)
    {
      await _dbContext.Classes.AddAsync(new Class
      {
        CourseId = c.Id,
        RegistrationDeadline = now.AddDays(3),
        MaxStudents = 30,
        TotalStudents = 5
      });
    }
    await _dbContext.SaveChangesAsync();

    var request = new ListCoursesRequest { PageNumber = 2, PageSize = 2 };

    // Act
    var result = await _queryHandler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Count, Is.EqualTo(1));
    Assert.That(result.Value.TotalCount, Is.EqualTo(3));
  }

  [Test]
  public async Task Handle_NotEnrolled_ShouldExcludeCoursesWithOnlyClosedOrFullClasses()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var openCourse = Builder<Course>.CreateNew().With(c => c.Name = "Open Course").Build();
    var closedOrFullCourse = Builder<Course>.CreateNew().With(c => c.Name = "Closed Or Full Course").Build();

    await _dbContext.Courses.AddRangeAsync(openCourse, closedOrFullCourse);

    // Open course: one class open & has capacity
    await _dbContext.Classes.AddAsync(new Class
    {
      CourseId = openCourse.Id,
      RegistrationDeadline = now.AddDays(2),
      MaxStudents = 20,
      TotalStudents = 10
    });

    // Closed or full course: one class with past deadline; one class full
    await _dbContext.Classes.AddRangeAsync(
      new Class { CourseId = closedOrFullCourse.Id, RegistrationDeadline = now.AddDays(-1), MaxStudents = 20, TotalStudents = 5 },
      new Class { CourseId = closedOrFullCourse.Id, RegistrationDeadline = now.AddDays(3), MaxStudents = 10, TotalStudents = 10 }
    );

    await _dbContext.SaveChangesAsync();

    var request = new ListCoursesRequest { PageNumber = 1, PageSize = 10, EnrolledClasses = new List<string>() };

    // Act
    var result = await _queryHandler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    var items = result.Value.Items;
    Assert.That(items.Any(c => c.Id == openCourse.Id), Is.True);
    Assert.That(items.Any(c => c.Id == closedOrFullCourse.Id), Is.False);
  }

  [Test]
  public async Task Handle_Enrolled_ShouldIncludeCourseEvenIfClosedOrFull()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var course = Builder<Course>.CreateNew().With(c => c.Name = "Enrolled Course").Build();
    await _dbContext.Courses.AddAsync(course);

    var closedFullClass = new Class { CourseId = course.Id, RegistrationDeadline = now.AddDays(-5), MaxStudents = 10, TotalStudents = 10 };
    await _dbContext.Classes.AddAsync(closedFullClass);
    await _dbContext.SaveChangesAsync();

    var request = new ListCoursesRequest
    {
      PageNumber = 1,
      PageSize = 10,
      EnrolledClasses = new List<string> { closedFullClass.Id }
    };

    // Act
    var result = await _queryHandler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Any(c => c.Id == course.Id), Is.True);
  }

  [Test]
  public async Task Handle_ShowAllTrue_ShouldReturnAllCourses_EvenIfClassesAreClosedOrFull()
  {
    // Arrange
    var now = DateTime.UtcNow;
    var openCourse = Builder<Course>.CreateNew().With(c => c.Name = "Open Course").Build();
    var closedOrFullCourse = Builder<Course>.CreateNew().With(c => c.Name = "Closed Or Full Course").Build();

    await AddCoursesToDatabase(openCourse, closedOrFullCourse);

    // Open course: one class open & has capacity
    await _dbContext.Classes.AddAsync(new Class
    {
      CourseId = openCourse.Id,
      RegistrationDeadline = now.AddDays(2),
      MaxStudents = 20,
      TotalStudents = 10
    });

    // Closed or full course: one class with past deadline; one class full
    await _dbContext.Classes.AddRangeAsync(
      new Class { CourseId = closedOrFullCourse.Id, RegistrationDeadline = now.AddDays(-1), MaxStudents = 20, TotalStudents = 5 },
      new Class { CourseId = closedOrFullCourse.Id, RegistrationDeadline = now.AddDays(3), MaxStudents = 10, TotalStudents = 10 }
    );

    await _dbContext.SaveChangesAsync();

    var request = new ListCoursesRequest { PageNumber = 1, PageSize = 10, ShowAll = true };

    // Act
    var result = await _queryHandler.Handle(request, CancellationToken.None);

    // Assert
    Assert.That(result.IsError, Is.False);
    var items = result.Value.Items;
    Assert.That(items.Any(c => c.Id == openCourse.Id), Is.True);
    Assert.That(items.Any(c => c.Id == closedOrFullCourse.Id), Is.True);
  }
}
