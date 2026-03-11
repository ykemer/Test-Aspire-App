using FluentValidation;

using Library.Validators;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommandValidator : AbstractValidator<EnrollStudentToClassCommand>
{
  public EnrollStudentToClassCommandValidator()
  {
    RuleFor(x => x.CourseId)
      .NotNull().WithMessage("Course Id is required.")
      .NotEmpty().WithMessage("Course Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Course id cannot be the empty GUID.");

    RuleFor(x => x.ClassId)
      .NotNull().WithMessage("Class Id is required.")
      .NotEmpty().WithMessage("Class Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Class id cannot be the empty GUID.");

    RuleFor(x => x.StudentId)
      .NotNull().WithMessage("Student Id is required.")
      .NotEmpty().WithMessage("Student Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Student id cannot be the empty GUID.");

    RuleFor(x => x.FirstName)
      .NotEmpty()
      .MaximumLength(100);

    RuleFor(x => x.LastName)
      .NotEmpty()
      .MaximumLength(100);
  }
}
