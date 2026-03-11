using FizzWare.NBuilder;

using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Courses.Application.Features.Courses.UpdateCourse;

[TestFixture]
public class UpdateCourseValidatorTests
{
  [SetUp]
  public void SetUp() => _validator = new UpdateCourseValidator();

  private UpdateCourseValidator _validator = null!;

  [Test]
  public void Validate_ValidInput_ShouldBeValid()
  {
    // Arrange
    var command =  Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "Valid Course Name")
      .With(c => c.Description = "Valid course description.")
      .Build();

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.True);
  }

  [Test]
  public void Validate_NameNull_ShouldBeInvalid()
  {
    // Arrange

    var command =  Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = null!)
      .With(c => c.Description = "Valid course description.")
      .Build();

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
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = string.Empty)
      .With(c => c.Description = "Valid course description.")
      .Build();

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
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "ab")
      .With(c => c.Description = "Valid course description.")
      .Build();

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(
      result.Errors.Any(e =>
        e.PropertyName == nameof(CreateCourseCommand.Name) && e.ErrorMessage == "Name is required."), Is.True);
  }

  [Test]
  public void Validate_NameTooLong_ShouldHaveMaxLengthMessage()
  {
    // Arrange (101 chars > MaximumLength 100)
    var longName = new string('n', 101);
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = longName)
      .With(c => c.Description = "Valid course description.")
      .Build();

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(
      result.Errors.Any(e =>
        e.PropertyName == nameof(CreateCourseCommand.Name) && e.ErrorMessage == "Name must not exceed 50 characters."),
      Is.True);
  }

  [Test]
  public void Validate_DescriptionNull_ShouldBeInvalid()
  {
    // Arrange
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "Valid Course Name")
      .With(c => c.Description = null!)
      .Build();

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
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "Valid Course Name")
      .With(c => c.Description = string.Empty)
      .Build();

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
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "Valid Course Name")
      .With(c => c.Description = "ab")
      .Build();

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(
      result.Errors.Any(e =>
        e.PropertyName == nameof(CreateCourseCommand.Description) && e.ErrorMessage == "Description is required."),
      Is.True);
  }

  [Test]
  public void Validate_DescriptionTooLong_ShouldHaveMaxLengthMessage()
  {
    // Arrange (501 chars)
    var longDesc = new string('d', 501);
    var command = Builder<UpdateCourseCommand>.CreateNew()
      .With(c => c.Id = Guid.NewGuid())
      .With(c => c.Name = "Valid Course Name")
      .With(c => c.Description = longDesc)
      .Build();

    // Act
    var result = _validator.Validate(command);

    // Assert
    Assert.That(result.IsValid, Is.False);
    Assert.That(
      result.Errors.Any(e =>
        e.PropertyName == nameof(CreateCourseCommand.Description) &&
        e.ErrorMessage == "Description must not exceed 500 characters."), Is.True);
  }
}
