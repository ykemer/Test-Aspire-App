using FluentValidation;

using Library.Validators;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommandValidator : AbstractValidator<UnenrollStudentFromClassCommand>
{
  public UnenrollStudentFromClassCommandValidator()
  {
    RuleFor(x => x.CourseId)
      .NotNull().WithMessage("Course Id is required.")
      .NotEmpty().WithMessage("Course Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Course Id cannot be the empty GUID.");

    RuleFor(x => x.ClassId)
      .NotNull().WithMessage("Class Id is required.")
      .NotEmpty().WithMessage("Class Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Class Id cannot be the empty GUID.");

    RuleFor(x => x.StudentId)
      .NotNull().WithMessage("Student Id is required.")
      .NotEmpty().WithMessage("Student Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Student Id cannot be the empty GUID.");
  }
}
