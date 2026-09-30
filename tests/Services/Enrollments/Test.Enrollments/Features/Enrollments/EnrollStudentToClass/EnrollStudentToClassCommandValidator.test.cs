using FluentValidation.TestHelper;

using Service.Enrollments.Features.Enrollments;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

namespace Test.Enrollments.Features.Enrollments.EnrollStudentToClass;

[TestFixture]
public class EnrollStudentToClassCommandValidatorTests
{
  private readonly EnrollStudentToClassCommandValidator _validator = new();

  private static EnrollStudentToClassCommand ValidCommand() =>
    new()
    {
      CourseId = Guid.NewGuid(),
      ClassId = Guid.NewGuid(),
      StudentId = Guid.NewGuid(),
      FirstName = "Jane",
      LastName = "Doe",
      IdempotencyKey = Guid.NewGuid()
    };

  [Test]
  public void ValidCommand_ShouldPass() =>
    _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void EmptyIds_ShouldFail()
  {
    var result = _validator.TestValidate(ValidCommand() with
    {
      CourseId = Guid.Empty, ClassId = Guid.Empty, StudentId = Guid.Empty, IdempotencyKey = Guid.Empty
    });

    result.ShouldHaveValidationErrorFor(x => x.CourseId);
    result.ShouldHaveValidationErrorFor(x => x.ClassId);
    result.ShouldHaveValidationErrorFor(x => x.StudentId);
    result.ShouldHaveValidationErrorFor(x => x.IdempotencyKey);
  }

  [Test]
  public void MissingNames_ShouldFail()
  {
    var result = _validator.TestValidate(ValidCommand() with { FirstName = "", LastName = "" });

    result.ShouldHaveValidationErrorFor(x => x.FirstName);
    result.ShouldHaveValidationErrorFor(x => x.LastName);
  }

  [Test]
  public void TooLongName_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { FirstName = new string('a', EnrollmentLimits.NameMaxLength + 1) })
      .ShouldHaveValidationErrorFor(x => x.FirstName);
}
