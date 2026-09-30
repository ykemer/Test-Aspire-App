using FluentValidation;

namespace Service.Enrollments.Features.Enrollments.EnrollStudentToClass;

public class EnrollStudentToClassCommandValidator : AbstractValidator<EnrollStudentToClassCommand>
{
  public EnrollStudentToClassCommandValidator()
  {
    RuleFor(command => command.CourseId).NotEmpty().WithMessage("Course id is required.");
    RuleFor(command => command.ClassId).NotEmpty().WithMessage("Class id is required.");
    RuleFor(command => command.StudentId).NotEmpty().WithMessage("Student id is required.");
    RuleFor(command => command.IdempotencyKey).NotEmpty().WithMessage("Idempotency key is required.");

    RuleFor(command => command.FirstName)
      .NotEmpty().WithMessage("First name is required.")
      .MaximumLength(EnrollmentLimits.NameMaxLength)
      .WithMessage($"First name must not exceed {EnrollmentLimits.NameMaxLength} characters.");

    RuleFor(command => command.LastName)
      .NotEmpty().WithMessage("Last name is required.")
      .MaximumLength(EnrollmentLimits.NameMaxLength)
      .WithMessage($"Last name must not exceed {EnrollmentLimits.NameMaxLength} characters.");
  }
}
