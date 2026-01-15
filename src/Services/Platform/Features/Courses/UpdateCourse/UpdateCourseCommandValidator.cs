using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseCommandValidator : Validator<UpdateCourseRequest>
{
  public UpdateCourseCommandValidator()
  {
    RuleFor(course => course.Name)
      .NotNull()
      .NotEmpty()
      .WithMessage("Name can not be empty")
      .MinimumLength(3)
      .WithMessage("Name must be at least 3 characters long")
      .MaximumLength(100)
      .WithMessage("Name can not exceed 100 characters");

    RuleFor(course => course.Description)
      .NotNull()
      .NotEmpty()
      .WithMessage("Description can not be empty")
      .MinimumLength(3)
      .WithMessage("Name must be at least 3 characters long")
      .MaximumLength(500)
      .WithMessage("Name can not exceed 100 characters");
  }
}
