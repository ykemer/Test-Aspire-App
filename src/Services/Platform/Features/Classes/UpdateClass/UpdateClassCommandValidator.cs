using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Classes.UpdateClass;

public class UpdateClassCommandValidator : Validator<UpdateClassRequest>
{
  public UpdateClassCommandValidator()
  {
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
