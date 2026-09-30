using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Service.Courses.Common.Database;
using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.CreateCourse;

namespace Test.Courses.Integration;

public class CourseNameUniquenessTests : PostgresTestBase
{
  private CreateCourseCommandHandler CreateHandler(ApplicationDbContext dbContext) =>
    new(dbContext, NullLogger<CreateCourseCommandHandler>.Instance, Clock);

  [Test]
  public async Task SameNameWithDifferentCase_IsRejected()
  {
    await CreateHandler(DbContext).Handle(new CreateCourseCommand("Biology", "Life"), default);

    var result = await CreateHandler(DbContext).Handle(new CreateCourseCommand("BIOLOGY", "Life again"), default);

    Assert.That(result.FirstError.Code, Is.EqualTo(CourseErrors.NameAlreadyTaken("BIOLOGY").Code));
  }

  [Test]
  public async Task DatabaseIndex_RejectsDuplicateEvenWhenCodeCheckIsSkipped()
  {
    // Simulates two requests that both passed the "name is free" check at the same moment.
    await CreateHandler(DbContext).Handle(new CreateCourseCommand("Physics", "Forces"), default);

    var insertDuplicate = () => DbContext.Database.ExecuteSqlRawAsync(
      "INSERT INTO \"Courses\" (\"Id\", \"Name\", \"Description\", \"TotalStudents\") " +
      "VALUES (gen_random_uuid(), 'physics', 'x', 0)");

    Assert.That(async () => await insertDuplicate(), Throws.Exception.Message.Contains("IX_Courses_Name_LowerCase_Unique"));
  }

  [Test]
  public async Task ParallelCreatesWithSameName_OnlyOneSucceeds()
  {
    var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(async _ =>
    {
      await using var context = PostgresDatabase.CreateDbContext();
      return await CreateHandler(context).Handle(new CreateCourseCommand("Chemistry", "Atoms"), default);
    }));

    Assert.That(results.Count(result => !result.IsError), Is.EqualTo(1));
    Assert.That(results.Where(result => result.IsError).Select(result => result.FirstError.Code),
      Is.All.EqualTo(CourseErrors.NameAlreadyTaken("Chemistry").Code));
  }
}
