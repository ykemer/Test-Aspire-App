using FluentValidation.TestHelper;

using Service.Students.Features;
using Service.Students.Features.CreateStudent;

using Test.Students.Setup;

namespace Test.Students.Features.CreateStudent;

[TestFixture]
public class CreateStudentCommandValidatorTests
{
  private CreateStudentCommandValidator _validator = null!;

  [SetUp]
  public void SetUp() => _validator = new CreateStudentCommandValidator(TestClock.Create());

  private static CreateStudentCommand ValidCommand() =>
    new()
    {
      Id = Guid.NewGuid(),
      FirstName = "Jane",
      LastName = "Doe",
      Email = "jane.doe@example.com",
      DateOfBirth = TestClock.Now.AddYears(-25)
    };

  [Test]
  public void ValidCommand_ShouldPass() =>
    _validator.TestValidate(ValidCommand()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void YoungStudent_ShouldPass_BecausePlatformOwnsRegistrationRules() =>
    _validator.TestValidate(ValidCommand() with { DateOfBirth = TestClock.Now.AddYears(-16) })
      .ShouldNotHaveAnyValidationErrors();

  [Test]
  public void EmptyId_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { Id = Guid.Empty }).ShouldHaveValidationErrorFor(x => x.Id);

  [TestCase("")]
  [TestCase(null)]
  public void MissingNames_ShouldFail(string? name)
  {
    var result = _validator.TestValidate(ValidCommand() with { FirstName = name!, LastName = name! });

    result.ShouldHaveValidationErrorFor(x => x.FirstName);
    result.ShouldHaveValidationErrorFor(x => x.LastName);
  }

  [Test]
  public void TooLongName_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { FirstName = new string('a', StudentLimits.NameMaxLength + 1) })
      .ShouldHaveValidationErrorFor(x => x.FirstName);

  [TestCase("")]
  [TestCase("not-an-email")]
  public void BadEmail_ShouldFail(string email) =>
    _validator.TestValidate(ValidCommand() with { Email = email }).ShouldHaveValidationErrorFor(x => x.Email);

  [Test]
  public void DateOfBirth_InTheFuture_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { DateOfBirth = TestClock.Now.AddDays(1) })
      .ShouldHaveValidationErrorFor(x => x.DateOfBirth);

  [Test]
  public void DateOfBirth_Missing_ShouldFail() =>
    _validator.TestValidate(ValidCommand() with { DateOfBirth = default })
      .ShouldHaveValidationErrorFor(x => x.DateOfBirth);
}
