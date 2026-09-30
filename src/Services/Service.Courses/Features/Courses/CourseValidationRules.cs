using FluentValidation;

namespace Service.Courses.Features.Courses;

/// <summary>
/// Validation rules shared by "create course" and "update course", so both always check the same things.
/// </summary>
public static class CourseValidationRules
{
  public static IRuleBuilderOptions<T, string> MustBeValidCourseName<T>(this IRuleBuilder<T, string> rule) =>
    rule
      .NotEmpty()
      .WithMessage("Name is required.")
      .Length(CourseLimits.NameMinLength, CourseLimits.NameMaxLength)
      .WithMessage(
        $"Name must be between {CourseLimits.NameMinLength} and {CourseLimits.NameMaxLength} characters.");

  public static IRuleBuilderOptions<T, string> MustBeValidCourseDescription<T>(this IRuleBuilder<T, string> rule) =>
    rule
      .NotEmpty()
      .WithMessage("Description is required.")
      .Length(CourseLimits.DescriptionMinLength, CourseLimits.DescriptionMaxLength)
      .WithMessage(
        $"Description must be between {CourseLimits.DescriptionMinLength} and {CourseLimits.DescriptionMaxLength} characters.");
}
