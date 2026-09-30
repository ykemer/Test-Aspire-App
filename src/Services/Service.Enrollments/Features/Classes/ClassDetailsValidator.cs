using FluentValidation;

namespace Service.Enrollments.Features.Classes;

/// <summary>
/// Checks that copied class data makes sense: ids are set, dates are in the right order, there is a seat.
/// <para>
/// It deliberately does NOT check that dates are in the future. The Courses service already checked that
/// when the class was saved. A message can arrive late or be replayed, and rejecting it here would leave
/// this copy out of sync with Courses forever.
/// </para>
/// </summary>
public class ClassDetailsValidator : AbstractValidator<IClassDetails>
{
  public ClassDetailsValidator()
  {
    RuleFor(details => details.Id).NotEmpty().WithMessage("Id is required.");
    RuleFor(details => details.CourseId).NotEmpty().WithMessage("Course id is required.");

    RuleFor(details => details.RegistrationDeadline)
      .LessThanOrEqualTo(details => details.CourseStartDate)
      .WithMessage("Registration deadline must not be after the course start date.");

    RuleFor(details => details.CourseStartDate)
      .LessThan(details => details.CourseEndDate)
      .WithMessage("Course start date must be before the course end date.");

    RuleFor(details => details.MaxStudents)
      .GreaterThan(0)
      .WithMessage("Maximum number of students must be greater than zero.");
  }
}
