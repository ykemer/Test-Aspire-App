using FluentValidation;

using Library.Validators;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommandValidator : AbstractValidator<UnenrollStudentFromClassCommand>
{
  public UnenrollStudentFromClassCommandValidator()
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
  }
}
