using Aspire_App.ApiService.Application.Students.Commands;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Students.Validators;

public class StudentCreateCommandValidator : Validator<StudentCreateCommand>
{
    public StudentCreateCommandValidator()
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
    }
}