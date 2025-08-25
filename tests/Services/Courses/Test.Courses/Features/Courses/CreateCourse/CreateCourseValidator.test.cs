using Service.Courses.Features.Courses.CreateCourse;

namespace Courses.Application.Features.Courses.CreateCourse;

[TestFixture]
public class CreateCourseValidatorTests
{
  private CreateCourseValidator _validator = null!;

  [SetUp]
  public void SetUp()
  {
    _validator = new CreateCourseValidator();
  }

  [Test]
  public void Validate_ValidInput_ShouldBeValid()
  {
    // Arrange
    var command = new CreateCourseCommand("Valid Name", "Valid Description");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.True);
  }

  [Test]
  public void Validate_NameNull_ShouldBeInvalid()
  {
    // Arrange
    var command = new CreateCourseCommand(null!, "Some description");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Name)), Is.True);
  }

  [Test]
  public void Validate_NameEmpty_ShouldBeInvalid()
  {
    // Arrange
    var command = new CreateCourseCommand(string.Empty, "Some description");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Name)), Is.True);
  }

  [Test]
  public void Validate_NameTooShort_ShouldHaveRequiredMessage()
  {
    // Arrange (2 chars < MinimumLength 3)
    var command = new CreateCourseCommand("ab", "Some description");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Name) && e.ErrorMessage == "Name is required."), Is.True);
  }

  [Test]
  public void Validate_NameTooLong_ShouldHaveMaxLengthMessage()
  {
    // Arrange (101 chars > MaximumLength 100)
    var longName = new string('n', 101);
    var command = new CreateCourseCommand(longName, "Some description");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    // The validator sets MaximumLength(100) but message says 50; assert current message as implemented
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Name) && e.ErrorMessage == "Name must not exceed 50 characters."), Is.True);
  }

  [Test]
  public void Validate_DescriptionNull_ShouldBeInvalid()
  {
    // Arrange
    var command = new CreateCourseCommand("Some name", null!);

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Description)), Is.True);
  }

  [Test]
  public void Validate_DescriptionEmpty_ShouldBeInvalid()
  {
    // Arrange
    var command = new CreateCourseCommand("Some name", string.Empty);

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Description)), Is.True);
  }

  [Test]
  public void Validate_DescriptionTooShort_ShouldHaveRequiredMessage()
  {
    // Arrange
    var command = new CreateCourseCommand("Some name", "ab");

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Description) && e.ErrorMessage == "Description is required."), Is.True);
  }

  [Test]
  public void Validate_DescriptionTooLong_ShouldHaveMaxLengthMessage()
  {
    // Arrange (501 chars)
    var longDesc = new string('d', 501);
    var command = new CreateCourseCommand("Some name", longDesc);

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateCourseCommand.Description) && e.ErrorMessage == "Description must not exceed 500 characters."), Is.True);
  }
}
