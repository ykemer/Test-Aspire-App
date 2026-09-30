using Contracts.Classes.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Classes;

/// <summary>
/// Rules for class dates and size, shared by "create class" and "update class"
/// (the update request inherits from the create request).
/// The same rules are checked again by the Courses service; checking here gives the admin an instant answer.
/// </summary>
[DontRegister] // a building block for the create/update validators, not a validator of its own
public class ClassScheduleRules : AbstractValidator<CreateClassRequest>
{
  public const int MaxStudentsLimit = 1000;

  private readonly TimeProvider _timeProvider;

  public ClassScheduleRules(TimeProvider timeProvider)
  {
    _timeProvider = timeProvider;

    RuleFor(request => request.RegistrationDeadline)
      .Must(BeInTheFuture).WithMessage("Registration deadline must be in the future.")
      .LessThan(request => request.CourseStartDate)
      .WithMessage("Registration deadline must be before the course start date.");

    RuleFor(request => request.CourseStartDate)
      .Must(BeInTheFuture).WithMessage("Course start date must be in the future.")
      .LessThan(request => request.CourseEndDate)
      .WithMessage("Course start date must be before the course end date.");

    RuleFor(request => request.CourseEndDate)
      .Must(BeInTheFuture).WithMessage("Course end date must be in the future.");

    RuleFor(request => request.MaxStudents)
      .InclusiveBetween(1, MaxStudentsLimit)
      .WithMessage($"Maximum number of students must be between 1 and {MaxStudentsLimit}.");
  }

  // Reads the clock on every validation, not once when the validator is created.
  private bool BeInTheFuture(DateTime date) => date > _timeProvider.GetUtcNow().UtcDateTime;
}
