using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Auth.UserLogin;

public class UserLoginCommandValidator : Validator<UserLoginRequest>
{
  public UserLoginCommandValidator()
  {
    RuleFor(i => i.Email)
      .NotNull()
      .NotEmpty()
      .WithMessage("Email can not be empty")
      .EmailAddress()
      .WithMessage("Email is not valid")
      .MaximumLength(256);

    RuleFor(i => i.Password)
      .NotNull()
      .NotEmpty()
      .WithMessage("Password can not be empty")
      .MinimumLength(6)
      .WithMessage("Password must be at least 6 characters long")
      .MaximumLength(100)
      .WithMessage("Password can not be longer than 100 characters");
  }
}
