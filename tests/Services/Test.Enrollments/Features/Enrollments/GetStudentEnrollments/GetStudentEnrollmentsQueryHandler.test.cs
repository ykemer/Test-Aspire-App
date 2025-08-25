using FizzWare.NBuilder;

using Service.Enrollments.Database;
using Service.Enrollments.Database.Entities;
using Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.GetStudentEnrollments;

[TestFixture]
public class GetStudentEnrollmentsQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetStudentEnrollmentsQueryHandler _handler;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new GetStudentEnrollmentsQueryHandler(_dbContext, Microsoft.Extensions.Logging.Abstractions.NullLogger<GetStudentEnrollmentsQueryHandler>.Instance);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForStudent_AllCourses_WhenCourseIdEmpty()
  {
    var e1 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-1").With(e => e.CourseId, "c1").Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-1").With(e => e.CourseId, "c2").Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-2").With(e => e.CourseId, "c1").Build();

    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetStudentEnrollmentsQuery("student-1", "");
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(2));
    Assert.That(result.Value.All(e => e.StudentId == "student-1"), Is.True);
  }

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForStudent_SpecificCourse_WhenCourseIdProvided()
  {
    var e1 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-1").With(e => e.CourseId, "c1").Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-1").With(e => e.CourseId, "c2").Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.StudentId, "student-2").With(e => e.CourseId, "c1").Build();

    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetStudentEnrollmentsQuery("student-1", "c2");
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(1));
    Assert.That(result.Value.All(e => e.StudentId == "student-1" && e.CourseId == "c2"), Is.True);
  }
}
