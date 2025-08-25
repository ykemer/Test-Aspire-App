using FluentValidation;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
  public CreateClassCommandValidator()
  {

    RuleFor(x => x.CourseId)
      .NotNull()
      .NotEmpty()
      .WithMessage("CourseId is required.");

    RuleFor(x => x.RegistrationDeadline)
      .NotNull()
      .NotEmpty()
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("Registration deadline must be in the future.")
      .LessThan(x => x.CourseStartDate)
      .WithMessage("Registration deadline must be before the course start date.")
      .LessThan(x => x.CourseEndDate)
      .WithMessage("Registration deadline must be before the course end date.");

    RuleFor(x => x.CourseStartDate)
      .NotNull()
      .NotEmpty()
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("Course start date must be in the future.")
      .LessThan(x => x.CourseEndDate)
      .WithMessage("Course start date must be before the course end date.");

    RuleFor(x => x.CourseEndDate)
      .NotNull()
      .NotEmpty()
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("Course end date must be in the future.");

    RuleFor(x => x.MaxStudents)
      .NotNull()
      .NotEmpty()
      .GreaterThan(0)
      .WithMessage("Maximum number of students must be greater than zero.");
  }
}
