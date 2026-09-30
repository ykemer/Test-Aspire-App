using FluentValidation;

namespace Service.Students.Features.CreateStudent;

/// <summary>
/// Checks that the user data copied from Platform makes sense: ids and names are set, the email looks
/// like an email, the birth date is in the past.
/// <para>
/// Business rules for registering (like a minimum age) belong to Platform, which already accepted this user.
/// Re-checking them here would only leave a registered user without a student record.
/// </para>
/// </summary>
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
  private readonly TimeProvider _timeProvider;

  public CreateStudentCommandValidator(TimeProvider timeProvider)
  {
    _timeProvider = timeProvider;

    RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required.");

    RuleFor(command => command.FirstName)
      .NotEmpty().WithMessage("First name is required.")
      .MaximumLength(StudentLimits.NameMaxLength)
      .WithMessage($"First name must not exceed {StudentLimits.NameMaxLength} characters.");

    RuleFor(command => command.LastName)
      .NotEmpty().WithMessage("Last name is required.")
      .MaximumLength(StudentLimits.NameMaxLength)
      .WithMessage($"Last name must not exceed {StudentLimits.NameMaxLength} characters.");

    RuleFor(command => command.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Email is not a valid email address.")
      .MaximumLength(StudentLimits.EmailMaxLength)
      .WithMessage($"Email must not exceed {StudentLimits.EmailMaxLength} characters.");

    RuleFor(command => command.DateOfBirth)
      .Must(BeInThePast)
      .WithMessage("Date of birth must be in the past.");
  }

  // Reads the clock on every validation, not once when the validator is created.
  private bool BeInThePast(DateTime date) => date != default && date < _timeProvider.GetUtcNow().UtcDateTime;
}
