using Service.Courses.Common.Database.Entities;
using Service.Courses.Features.Courses.ListCourses;

namespace Test.Courses.Integration;

public class ListCoursesSearchTests : PostgresTestBase
{
  private async Task AddCourse(string name, string description)
  {
    DbContext.Courses.Add(new Course { Name = name, Description = description });
    await DbContext.SaveChangesAsync();
  }

  [Test]
  public async Task Search_ReturnsOnlyMatches_BestMatchFirst()
  {
    await AddCourse("Cooking basics", "Learn to cook pasta");
    await AddCourse("Pasta pasta pasta", "Everything about pasta");
    await AddCourse("Gardening", "Grow tomatoes");
    var handler = new ListCoursesQueryHandler(DbContext, Clock);

    var result = await handler.Handle(new ListCoursesQuery { Query = "pasta", ShowAll = true }, default);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items.Select(course => course.Name),
      Is.EqualTo(new[] { "Pasta pasta pasta", "Cooking basics" }));
    Assert.That(result.Value.TotalCount, Is.EqualTo(2));
  }

  [Test]
  public async Task Search_WithSpecialCharacters_DoesNotCrash()
  {
    await AddCourse("Math", "Numbers");
    var handler = new ListCoursesQueryHandler(DbContext, Clock);

    var result = await handler.Handle(new ListCoursesQuery { Query = "'); DROP TABLE \"Courses\"; --", ShowAll = true },
      default);

    Assert.That(result.IsError, Is.False);
    Assert.That(result.Value.Items, Is.Empty);
  }
}
