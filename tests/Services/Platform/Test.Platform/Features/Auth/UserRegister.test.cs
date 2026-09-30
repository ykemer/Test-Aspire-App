using Contracts.Users.Requests;

using ErrorOr;

using FluentValidation.TestHelper;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;

using Platform.Common.Auth;
using Platform.Features.Auth.UserRegister;

namespace Test.Platform.Features.Auth;

[TestFixture]
public class UserRegisterTests
{
  private static readonly DateTime s_today = new(2030, 6, 15, 0, 0, 0, DateTimeKind.Utc);

  private UserRegisterCommandValidator _validator = null!;

  [SetUp]
  public void SetUp() => _validator = new UserRegisterCommandValidator(new FakeTimeProvider(s_today.AddHours(10)));

  private static UserRegisterRequest ValidRequest() =>
    new()
    {
      FirstName = "Jane",
      LastName = "Doe",
      Email = "jane@example.com",
      Password = "Secret1!",
      RepeatPassword = "Secret1!",
      DateOfBirth = s_today.AddYears(-25)
    };

  [Test]
  public void ValidRequest_ShouldPass() =>
    _validator.TestValidate(ValidRequest()).ShouldNotHaveAnyValidationErrors();

  [Test]
  public void Exactly18Today_ShouldPass()
  {
    var request = ValidRequest();
    request.DateOfBirth = s_today.AddYears(-18);

    _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
  }

  [Test]
  public void Turning18Tomorrow_ShouldFail()
  {
    // The old check compared only years, so this person counted as 18 already.
    var request = ValidRequest();
    request.DateOfBirth = s_today.AddYears(-18).AddDays(1);

    _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.DateOfBirth);
  }

  [TestCase("short1!", Description = "too short")]
  [TestCase("secret1!", Description = "no uppercase")]
  [TestCase("SECRET1!", Description = "no lowercase")]
  [TestCase("Secret!!", Description = "no digit")]
  [TestCase("Secret12", Description = "no symbol")]
  public void WeakPassword_ShouldFail(string password)
  {
    var request = ValidRequest();
    request.Password = password;
    request.RepeatPassword = password;

    _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Test]
  public void MissingPassword_FailsCleanly_WithoutCrashing()
  {
    var request = ValidRequest();
    request.Password = null!;

    _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Password);
  }

  [Test]
  public void PasswordsThatDoNotMatch_ShouldFail()
  {
    var request = ValidRequest();
    request.RepeatPassword = "Other1!!";

    _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.RepeatPassword);
  }

  [Test]
  public void ValidatorPasswordRules_MatchIdentityPasswordRules()
  {
    var identityOptions = new PasswordOptions();
    PasswordRules.ApplyTo(identityOptions);

    Assert.Multiple(() =>
    {
      Assert.That(identityOptions.RequiredLength, Is.EqualTo(PasswordRules.MinLength));
      Assert.That(identityOptions.RequireDigit && identityOptions.RequireLowercase
                                               && identityOptions.RequireUppercase
                                               && identityOptions.RequireNonAlphanumeric, Is.True);
    });
  }

  [Test]
  public void DuplicateEmailFromIdentity_BecomesConflict()
  {
    var errors = new[] { new IdentityError { Code = "DuplicateUserName", Description = "taken" } }.ToApiErrors();

    Assert.That(errors.Single().Type, Is.EqualTo(ErrorType.Conflict));
  }

  [Test]
  public void OtherIdentityErrors_BecomeValidationErrors_WithIdentityMessage()
  {
    var errors = new[] { new IdentityError { Code = "PasswordTooShort", Description = "Too short." } }.ToApiErrors();

    Assert.That(errors.Single().Type, Is.EqualTo(ErrorType.Validation));
    Assert.That(errors.Single().Description, Is.EqualTo("Too short."));
  }
}
