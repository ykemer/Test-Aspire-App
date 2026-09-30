using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

using Platform.Common.Auth;

namespace Platform.Features.Auth.UserRegister;

/// <summary>
/// Registration rules. This is the ONE place where the business rules for new users live
/// (for example the minimum age); other services trust users that Platform accepted.
/// </summary>
public class UserRegisterCommandValidator : Validator<UserRegisterRequest>
{
  public const int NameMinLength = 3;
  public const int NameMaxLength = 50;
  public const int EmailMaxLength = 100;
  public const int MinimumAge = 18;
  public const int MaximumAge = 100;

  private readonly TimeProvider _timeProvider;

  // FastEndpoints creates validators once; the clock is read on every validation, not at creation.
  public UserRegisterCommandValidator() : this(TimeProvider.System)
  {
  }

  public UserRegisterCommandValidator(TimeProvider timeProvider)
  {
    _timeProvider = timeProvider;

    RuleFor(request => request.FirstName)
      .NotEmpty().WithMessage("First name is required.")
      .Length(NameMinLength, NameMaxLength)
      .WithMessage($"First name must be between {NameMinLength} and {NameMaxLength} characters.");

    RuleFor(request => request.LastName)
      .NotEmpty().WithMessage("Last name is required.")
      .Length(NameMinLength, NameMaxLength)
      .WithMessage($"Last name must be between {NameMinLength} and {NameMaxLength} characters.");

    RuleFor(request => request.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Email is not valid.")
      .MaximumLength(EmailMaxLength).WithMessage($"Email must not exceed {EmailMaxLength} characters.");

    RuleFor(request => request.DateOfBirth)
      .Must(BeOldEnough).WithMessage($"You must be at least {MinimumAge} years old.")
      .Must(BeRealistic).WithMessage("Date of birth is not valid.");

    RuleFor(request => request.Password)
      .Cascade(CascadeMode.Stop) // stop at the first failed check, so the checks below never see null
      .NotEmpty().WithMessage("Password is required.")
      .Length(PasswordRules.MinLength, PasswordRules.MaxLength)
      .WithMessage($"Password must be between {PasswordRules.MinLength} and {PasswordRules.MaxLength} characters.")
      .Must(password => password.Any(char.IsDigit)).WithMessage("Password must contain a digit.")
      .Must(password => password.Any(char.IsLower)).WithMessage("Password must contain a lowercase letter.")
      .Must(password => password.Any(char.IsUpper)).WithMessage("Password must contain an uppercase letter.")
      .Must(password => password.Any(character => !char.IsLetterOrDigit(character)))
      .WithMessage("Password must contain a symbol (for example ! or #).");

    RuleFor(request => request.RepeatPassword)
      .Equal(request => request.Password).WithMessage("Passwords do not match.");
  }

  private DateTime Today => _timeProvider.GetUtcNow().UtcDateTime.Date;

  // Compares full dates, not just years: someone born later this year is not 18 yet.
  private bool BeOldEnough(DateTime dateOfBirth) => dateOfBirth.Date <= Today.AddYears(-MinimumAge);

  private bool BeRealistic(DateTime dateOfBirth) => dateOfBirth.Date > Today.AddYears(-MaximumAge);
}
