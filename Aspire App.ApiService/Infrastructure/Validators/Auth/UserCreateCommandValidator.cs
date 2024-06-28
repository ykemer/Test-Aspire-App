using Contracts.Auth.Requests;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Infrastructure.Validators.Auth;

public class UserCreateCommandValidator : Validator<UserRegisterRequest>
{
    public UserCreateCommandValidator()
    {
        RuleFor(student => student.FirstName).NotEmpty().WithMessage("First Name can not be empty");
        RuleFor(student => student.LastName).NotEmpty().WithMessage("Last Name can not be empty");
        RuleFor(student => student.Email)
            .NotEmpty()
            .WithMessage("Email can not be empty")
            .EmailAddress()
            .WithMessage("Wrong email format");

        RuleFor(student => student.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth can not be empty")
            .Must(i => i.Year < DateTime.Now.Year - 18)
            .WithMessage("Student must be at least 18 years old")
            .Must(i => i.Year > DateTime.Now.Year - 100)
            .WithMessage("Wrong date");


        RuleFor(i => i.Password).NotEmpty()
            .Must(i => i.Length > 6)
            .WithMessage("Password must be at least 6 characters long")
            .Must(i => i.Any(char.IsDigit))
            .WithMessage("Password must contain at least one digit");

        RuleFor(i => i.RepeatPassword).Equal(i => i.Password);

        RuleFor(i => i.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth can not be empty")
            .Must(i => i.Year < DateTime.Now.Year - 18)
            .WithMessage("Student must be at least 18 years old")
            .Must(i => i.Year > DateTime.Now.Year - 100)
            .WithMessage("Wrong date");
    }
}