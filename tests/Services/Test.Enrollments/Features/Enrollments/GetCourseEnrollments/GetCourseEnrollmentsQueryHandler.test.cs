using FizzWare.NBuilder;

using Service.Enrollments.Database;
using Service.Enrollments.Database.Entities;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.GetCourseEnrollments;

[TestFixture]
public class GetCourseEnrollmentsQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetCourseEnrollmentsQueryHandler _handler;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new GetCourseEnrollmentsQueryHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForCourse()
  {
    var e1 = Builder<Enrollment>.CreateNew().With(e => e.CourseId, "course-1").Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.CourseId, "course-1").Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.CourseId, "course-2").Build();

    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetCourseEnrollmentsQuery("course-1");
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(2));
    Assert.That(result.Value.All(e => e.CourseId == "course-1"), Is.True);
  }
}
