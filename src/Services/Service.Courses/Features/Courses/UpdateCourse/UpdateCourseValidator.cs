using FluentValidation;

using Service.Courses.Features.Courses.CreateCourse;

namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseValidator : AbstractValidator<CreateCourseCommand>
{
  public UpdateCourseValidator()
  {
    RuleFor(x => x.Name)
      .NotNull()
      .NotEmpty()
      .MinimumLength(3)
      .WithMessage("Name is required.")
      .MaximumLength(100)
      .WithMessage("Name must not exceed 50 characters.");

    RuleFor(x => x.Description)
      .NotNull()
      .NotEmpty()
      .MinimumLength(3)
      .WithMessage("Description is required.")
      .MaximumLength(500)
      .WithMessage("Description must not exceed 500 characters.");
  }
}
