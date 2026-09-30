using FluentValidation;

namespace Service.Courses.Features.Classes.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
  public CreateClassCommandValidator(TimeProvider timeProvider)
  {
    RuleFor(command => command.CourseId).NotEmpty().WithMessage("CourseId is required.");
    Include(new ClassDetailsValidator(timeProvider));
  }
}
