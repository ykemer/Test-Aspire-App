using FluentValidation.TestHelper;

using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.ListCourses;

namespace Test.Courses.Features.Courses.ListCourses;

[TestFixture]
public class ListCoursesQueryValidatorTests
{
  private readonly ListCoursesQueryValidator _validator = new();

  [Test]
  public void DefaultQuery_ShouldPass() =>
    _validator.TestValidate(new ListCoursesQuery()).ShouldNotHaveAnyValidationErrors();

  [TestCase(0)]
  [TestCase(-3)]
  public void PageNumberBelowOne_ShouldFail(int pageNumber) =>
    _validator.TestValidate(new ListCoursesQuery { PageNumber = pageNumber })
      .ShouldHaveValidationErrorFor(x => x.PageNumber);

  [TestCase(0)]
  [TestCase(-3)]
  public void PageSizeBelowOne_ShouldFail(int pageSize) =>
    _validator.TestValidate(new ListCoursesQuery { PageSize = pageSize })
      .ShouldHaveValidationErrorFor(x => x.PageSize);

  [Test]
  public void SearchTextTooLong_ShouldFail() =>
    _validator.TestValidate(new ListCoursesQuery { Query = new string('q', CourseLimits.SearchQueryMaxLength + 1) })
      .ShouldHaveValidationErrorFor(x => x.Query);
}
