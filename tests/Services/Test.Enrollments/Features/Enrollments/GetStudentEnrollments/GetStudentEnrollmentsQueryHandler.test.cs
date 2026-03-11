using FizzWare.NBuilder;

using Microsoft.Extensions.Logging.Abstractions;

using Service.Enrollments.Common.Database;
using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.GetStudentEnrollments;

[TestFixture]
public class GetStudentEnrollmentsQueryHandlerTests
{
  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new GetStudentEnrollmentsQueryHandler(_dbContext,
      NullLogger<GetStudentEnrollmentsQueryHandler>.Instance);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  private ApplicationDbContext _dbContext;
  private GetStudentEnrollmentsQueryHandler _handler;

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForStudent_AllCourses_WhenCourseIdEmpty()
  {
    var studentId = Guid.NewGuid();
    var courseId1 = Guid.NewGuid();
    var courseId2 = Guid.NewGuid();
    var otherStudentId = Guid.NewGuid();

    var e1 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, studentId).With(e => e.CourseId, courseId1).Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, studentId).With(e => e.CourseId, courseId2).Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.StudentId, otherStudentId).With(e => e.CourseId, courseId1)
      .Build();

    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetStudentEnrollmentsQuery(studentId, null);
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(2));
    Assert.That(result.Value.All(e => e.StudentId == studentId), Is.True);
  }

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForStudent_SpecificCourse_WhenCourseIdProvided()
  {
    var studentId = Guid.NewGuid();
    var courseId1 = Guid.NewGuid();
    var courseId2 = Guid.NewGuid();
    var otherStudentId = Guid.NewGuid();

    var e1 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, studentId).With(e => e.CourseId, courseId1).Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.StudentId, studentId).With(e => e.CourseId, courseId2).Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.StudentId, otherStudentId).With(e => e.CourseId, courseId1)
      .Build();

    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetStudentEnrollmentsQuery(studentId, courseId2);
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(1));
    Assert.That(result.Value.All(e => e.StudentId == studentId && e.CourseId == courseId2), Is.True);
  }
}
