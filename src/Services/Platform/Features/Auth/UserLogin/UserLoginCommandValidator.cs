using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

using Platform.Common.Auth;

namespace Platform.Features.Auth.UserLogin;

/// <summary>
/// Only checks that something was typed. The password policy is NOT checked here:
/// users with older, weaker passwords must still be able to sign in.
/// </summary>
public class UserLoginCommandValidator : Validator<UserLoginRequest>
{
  public UserLoginCommandValidator()
  {
    RuleFor(request => request.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("Email is not valid.")
      .MaximumLength(256);

    RuleFor(request => request.Password)
      .NotEmpty().WithMessage("Password is required.")
      .MaximumLength(PasswordRules.MaxLength);
  }
}
