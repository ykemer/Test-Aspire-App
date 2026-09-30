using FluentValidation.TestHelper;

using Service.Courses.Features.Courses;
using Service.Courses.Features.Courses.CreateCourse;

namespace Test.Courses.Features.Courses.CreateCourse;

[TestFixture]
public class CreateCourseValidatorTests
{
  private const string ValidName = "Valid Name";
  private const string ValidDescription = "Valid Description";

  private CreateCourseValidator _validator = null!;

  [SetUp]
  public void SetUp() => _validator = new CreateCourseValidator();

  [Test]
  public void ValidInput_ShouldPass() =>
    _validator.TestValidate(new CreateCourseCommand(ValidName, ValidDescription)).ShouldNotHaveAnyValidationErrors();

  [TestCase(null)]
  [TestCase("")]
  public void MissingName_ShouldFail_WithRequiredMessage(string? name) =>
    _validator.TestValidate(new CreateCourseCommand(name!, ValidDescription))
      .ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Name is required.");

  [Test]
  public void NameTooShort_ShouldFail() =>
    _validator.TestValidate(new CreateCourseCommand("ab", ValidDescription))
      .ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Name must be between 3 and 100 characters.");

  [Test]
  public void NameTooLong_ShouldFail() =>
    _validator.TestValidate(new CreateCourseCommand(new string('n', CourseLimits.NameMaxLength + 1), ValidDescription))
      .ShouldHaveValidationErrorFor(x => x.Name)
      .WithErrorMessage("Name must be between 3 and 100 characters.");

  [Test]
  public void NameAtMaxLength_ShouldPass() =>
    _validator.TestValidate(new CreateCourseCommand(new string('n', CourseLimits.NameMaxLength), ValidDescription))
      .ShouldNotHaveValidationErrorFor(x => x.Name);

  [TestCase(null)]
  [TestCase("")]
  public void MissingDescription_ShouldFail_WithRequiredMessage(string? description) =>
    _validator.TestValidate(new CreateCourseCommand(ValidName, description!))
      .ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Description is required.");

  [Test]
  public void DescriptionTooShort_ShouldFail() =>
    _validator.TestValidate(new CreateCourseCommand(ValidName, "ab"))
      .ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Description must be between 3 and 500 characters.");

  [Test]
  public void DescriptionTooLong_ShouldFail() =>
    _validator.TestValidate(
        new CreateCourseCommand(ValidName, new string('d', CourseLimits.DescriptionMaxLength + 1)))
      .ShouldHaveValidationErrorFor(x => x.Description)
      .WithErrorMessage("Description must be between 3 and 500 characters.");
}
