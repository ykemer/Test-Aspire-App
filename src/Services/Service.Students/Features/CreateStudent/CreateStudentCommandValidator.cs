using FluentValidation;

namespace Service.Students.Features.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
  public CreateStudentCommandValidator()
  {
    RuleFor(x => x.Id)
      .NotNull().WithMessage("Id is required.")
      .NotEmpty().WithMessage("Id is required.")
      .NotEqual("00000000-0000-0000-0000-000000000000")
      .WithMessage("Id cannot be the empty GUID.");

    RuleFor(student => student.FirstName)
      .NotNull()
      .NotEmpty()
      .WithMessage("First Name can not be empty");

    RuleFor(student => student.LastName)
      .NotNull()
      .NotEmpty()
      .WithMessage("Last Name can not be empty");

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
  }
}
