using FluentValidation;

namespace Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

public class UnenrollStudentFromClassCommandValidator : AbstractValidator<UnenrollStudentFromClassCommand>
{
  public UnenrollStudentFromClassCommandValidator()
  {
    RuleFor(command => command.CourseId).NotEmpty().WithMessage("Course id is required.");
    RuleFor(command => command.ClassId).NotEmpty().WithMessage("Class id is required.");
    RuleFor(command => command.StudentId).NotEmpty().WithMessage("Student id is required.");
    RuleFor(command => command.IdempotencyKey).NotEmpty().WithMessage("Idempotency key is required.");
  }
}
