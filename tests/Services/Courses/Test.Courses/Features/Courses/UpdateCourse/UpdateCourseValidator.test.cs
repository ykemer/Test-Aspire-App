using FluentValidation.TestHelper;

using Service.Courses.Features.Courses.UpdateCourse;

namespace Test.Courses.Features.Courses.UpdateCourse;

/// <summary>
/// Name/description rules are shared with "create course" and fully tested there.
/// These tests only check that the update validator uses them, plus the Id rule.
/// </summary>
[TestFixture]
public class UpdateCourseValidatorTests
{
  private UpdateCourseValidator _validator = null!;

  [SetUp]
  public void SetUp() => _validator = new UpdateCourseValidator();

  private static UpdateCourseCommand ValidCommand() =>
    new() { Id = Guid.NewGuid(), Name = "Valid Course Name", Description = "Valid course description." };

  [Test]
  public void ValidInput_ShouldPass() =>
    _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void EmptyId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Id = Guid.Empty })
      .ShouldHaveValidationErrorFor(x => x.Id);

  [Test]
  public void InvalidName_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Name = "ab" })
      .ShouldHaveValidationErrorFor(x => x.Name);

  [Test]
  public void InvalidDescription_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Description = string.Empty })
      .ShouldHaveValidationErrorFor(x => x.Description);
}
