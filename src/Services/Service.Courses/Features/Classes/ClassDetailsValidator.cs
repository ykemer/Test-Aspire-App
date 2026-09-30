using FluentValidation;

namespace Service.Courses.Features.Classes;

/// <summary>
/// Rules for class dates and size: everything is in the future,
/// registration closes before the class starts, the class starts before it ends,
/// and there is at least one seat.
/// </summary>
public class ClassDetailsValidator : AbstractValidator<IClassDetails>
{
  private readonly TimeProvider _timeProvider;

  public ClassDetailsValidator(TimeProvider timeProvider)
  {
    _timeProvider = timeProvider;

    RuleFor(details => details.RegistrationDeadline)
      .Must(BeInTheFuture)
      .WithMessage("Registration deadline must be in the future.")
      .LessThan(details => details.CourseStartDate)
      .WithMessage("Registration deadline must be before the course start date.");

    RuleFor(details => details.CourseStartDate)
      .Must(BeInTheFuture)
      .WithMessage("Course start date must be in the future.")
      .LessThan(details => details.CourseEndDate)
      .WithMessage("Course start date must be before the course end date.");

    RuleFor(details => details.CourseEndDate)
      .Must(BeInTheFuture)
      .WithMessage("Course end date must be in the future.");

    RuleFor(details => details.MaxStudents)
      .GreaterThan(0)
      .WithMessage("Maximum number of students must be greater than zero.");
  }

  // Reads the clock on every validation, not once when the validator is created.
  private bool BeInTheFuture(DateTime date) => date > _timeProvider.GetUtcNow().UtcDateTime;
}
