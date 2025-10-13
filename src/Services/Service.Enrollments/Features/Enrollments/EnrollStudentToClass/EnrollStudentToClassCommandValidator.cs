using FluentValidation;

using Library.Validators;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommandValidator : AbstractValidator<EnrollStudentToClassCommand>
{
  public EnrollStudentToClassCommandValidator()
  {
    RuleFor(x => x.CourseId)
      .NotEmpty()
      .MaximumLength(50)
      .Must(Validators.IsValidGuid).WithMessage("Must be a valid GUID.");

    RuleFor(x => x.ClassId)
      .NotEmpty()
      .MaximumLength(50)
      .Must(Validators.IsValidGuid).WithMessage("Must be a valid GUID.");

    RuleFor(x => x.StudentId)
      .NotEmpty()
      .MaximumLength(50)
      .Must(Validators.IsValidGuid).WithMessage("Must be a valid GUID.");

    RuleFor(x => x.FirstName)
      .NotEmpty()
      .MaximumLength(100);

    RuleFor(x => x.LastName)
      .NotEmpty()
      .MaximumLength(100);
  }
}
