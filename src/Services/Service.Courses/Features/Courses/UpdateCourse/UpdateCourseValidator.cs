using FluentValidation;

namespace Service.Courses.Features.Courses.UpdateCourse;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
{
  public UpdateCourseValidator()
  {
    RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required.");
    RuleFor(command => command.Name).MustBeValidCourseName();
    RuleFor(command => command.Description).MustBeValidCourseDescription();
  }
}
