using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Features.Classes;
using Service.Enrollments.Features.Classes.CourseDeleted;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Classes.CourseDeleted;

[TestFixture]
public class DeleteClassesByCourseIdCommandHandlerTests
{
  private ApplicationDbContext _dbContext = null!;
  private DeleteClassesByCourseIdCommandHandler _handler = null!;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new DeleteClassesByCourseIdCommandHandler(_dbContext,
      NullLogger<DeleteClassesByCourseIdCommandHandler>.Instance);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldSucceed_WhenCourseHasNoClassesHere()
  {
    var result = await _handler.Handle(new DeleteClassesByCourseIdCommand(Guid.NewGuid()), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
  }

  [Test]
  public async Task Handle_ShouldRefuse_WhenAnyClassHasEnrollments()
  {
    var courseId = Guid.NewGuid();
    var classWithStudent = TestData.OpenClass(courseId);
    _dbContext.Classes.AddRange(classWithStudent, TestData.OpenClass(courseId));
    _dbContext.Enrollments.Add(TestData.EnrollmentIn(classWithStudent));
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteClassesByCourseIdCommand(courseId), CancellationToken.None);

    Assert.That(result.FirstError.Code, Is.EqualTo(ClassErrors.CourseHasEnrollments(courseId).Code));
    Assert.That(_dbContext.Classes.Count(), Is.EqualTo(2));
  }

  [Test]
  public async Task Handle_ShouldDeleteOnlyThatCoursesClasses()
  {
    var courseId = Guid.NewGuid();
    var otherCourseClass = TestData.OpenClass();
    _dbContext.Classes.AddRange(TestData.OpenClass(courseId), TestData.OpenClass(courseId), otherCourseClass);
    await _dbContext.SaveChangesAsync();

    var result = await _handler.Handle(new DeleteClassesByCourseIdCommand(courseId), CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(_dbContext.Classes.Select(c => c.Id), Is.EqualTo(new[] { otherCourseClass.Id }));
  }
}
