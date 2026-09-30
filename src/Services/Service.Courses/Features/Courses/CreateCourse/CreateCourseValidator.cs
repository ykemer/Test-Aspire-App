using FluentValidation;

namespace Service.Courses.Features.Courses.CreateCourse;

public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
  public CreateCourseValidator()
  {
    RuleFor(command => command.Name).MustBeValidCourseName();
    RuleFor(command => command.Description).MustBeValidCourseDescription();
  }
}
