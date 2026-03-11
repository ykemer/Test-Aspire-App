using FluentValidation;

namespace Service.Courses.Features.Classes.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
  public UpdateClassCommandValidator()
  {
    RuleFor(x => x.CourseId)
      .NotNull().WithMessage("CourseId is required.")
      .NotEmpty().WithMessage("CourseId is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("CourseId cannot be the empty GUID.");

    RuleFor(x => x.Id)
      .NotNull().WithMessage("Id is required.")
      .NotEmpty().WithMessage("Id is required.")
      .NotEqual(Guid.Empty)
      .WithMessage("Id cannot be the empty GUID.");

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
