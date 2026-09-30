using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Courses;

/// <summary>
/// Rules for course name and description, shared by "create course" and "update course"
/// (the update request inherits from the create request). Limits match the Courses service.
/// </summary>
[DontRegister] // a building block for the create/update validators, not a validator of its own
public class CourseDetailsRules : AbstractValidator<CreateCourseRequest>
{
  public const int NameMinLength = 3;
  public const int NameMaxLength = 100;
  public const int DescriptionMinLength = 3;
  public const int DescriptionMaxLength = 500;

  public CourseDetailsRules()
  {
    RuleFor(request => request.Name)
      .NotEmpty().WithMessage("Name is required.")
      .Length(NameMinLength, NameMaxLength)
      .WithMessage($"Name must be between {NameMinLength} and {NameMaxLength} characters.");

    RuleFor(request => request.Description)
      .NotEmpty().WithMessage("Description is required.")
      .Length(DescriptionMinLength, DescriptionMaxLength)
      .WithMessage($"Description must be between {DescriptionMinLength} and {DescriptionMaxLength} characters.");
  }
}
