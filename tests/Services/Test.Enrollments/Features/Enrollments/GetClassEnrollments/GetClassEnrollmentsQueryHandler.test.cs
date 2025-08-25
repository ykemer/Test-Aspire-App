using FizzWare.NBuilder;

using Service.Enrollments.Database;
using Service.Enrollments.Database.Entities;
using Service.Enrollments.Features.Enrollments.GetClassEnrollments;

using Test.Enrollments.Setup;

namespace Test.Enrollments.Features.Enrollments.GetClassEnrollments;

[TestFixture]
public class GetClassEnrollmentsQueryHandlerTests
{
  private ApplicationDbContext _dbContext;
  private GetClassEnrollmentsQueryHandler _handler;

  [SetUp]
  public void SetUp()
  {
    _dbContext = ApplicationDbContextCreator.GetDbContext();
    _handler = new GetClassEnrollmentsQueryHandler(_dbContext);
  }

  [TearDown]
  public void TearDown() => _dbContext.Dispose();

  [Test]
  public async Task Handle_ShouldReturnEnrollmentsForClass()
  {
    var cls1 = Builder<Class>.CreateNew().Build();
    var cls2 = Builder<Class>.CreateNew().Build();

    var e1 = Builder<Enrollment>.CreateNew().With(e => e.CourseId, cls1.CourseId).With(e => e.ClassId, cls1.Id).Build();
    var e2 = Builder<Enrollment>.CreateNew().With(e => e.CourseId, cls1.CourseId).With(e => e.ClassId, cls1.Id).Build();
    var eOther = Builder<Enrollment>.CreateNew().With(e => e.CourseId, cls2.CourseId).With(e => e.ClassId, cls2.Id).Build();

    await _dbContext.Classes.AddRangeAsync(cls1, cls2);
    await _dbContext.Enrollments.AddRangeAsync(e1, e2, eOther);
    await _dbContext.SaveChangesAsync();

    var query = new GetClassEnrollmentsQuery(cls1.CourseId, cls1.Id);
    var result = await _handler.Handle(query, CancellationToken.None);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Count, Is.EqualTo(2));
    Assert.That(result.Value.All(e => e.ClassId == cls1.Id && e.CourseId == cls1.CourseId), Is.True);
  }
}
