using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Auth.UserRegister;

public class UserRegisterCommandValidator : Validator<UserRegisterRequest>
{
  public UserRegisterCommandValidator()
  {
    RuleFor(student => student.FirstName)
      .NotNull()
      .NotEmpty()
      .WithMessage("First Name can not be empty")
      .MinimumLength(3)
      .WithMessage("First Name must be at least 3 characters long")
      .MaximumLength(50)
      .WithMessage("First Name can not be longer than 50 characters");

    RuleFor(student => student.LastName)
      .NotNull()
      .NotEmpty()
      .WithMessage("Last Name can not be empty")
      .MinimumLength(3)
      .WithMessage("Last Name must be at least 3 characters long")
      .MaximumLength(50)
      .WithMessage("Last Name can not be longer than 50 characters");

    RuleFor(student => student.Email)
      .NotNull()
      .NotEmpty()
      .WithMessage("Email can not be empty")
      .EmailAddress()
      .WithMessage("Wrong email format");

    RuleFor(student => student.DateOfBirth)
      .NotNull()
      .NotEmpty()
      .WithMessage("Date of birth can not be empty")
      .Must(i => i.Year < DateTime.Now.Year - 18)
      .WithMessage("Student must be at least 18 years old")
      .Must(i => i.Year > DateTime.Now.Year - 100)
      .WithMessage("Wrong date");

    RuleFor(student => student.Password)
      .NotNull()
      .NotEmpty()
      .WithMessage("Password can not be empty")
      .MinimumLength(6)
      .WithMessage("Password must be at least 6 characters long")
      .Must(i => i != null && i.Any(char.IsDigit))
      .WithMessage("Password must contain at least one digit");

    RuleFor(i => i.RepeatPassword)
      .NotNull()
      .NotEmpty()
      .WithMessage("Repeat password can not be empty")
      .Equal(i => i.Password)
      .WithMessage("Passwords can't be different");
  }
}
