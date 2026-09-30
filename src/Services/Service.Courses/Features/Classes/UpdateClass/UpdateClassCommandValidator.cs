using FluentValidation;

namespace Service.Courses.Features.Classes.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
  public UpdateClassCommandValidator(TimeProvider timeProvider)
  {
    RuleFor(command => command.Id).NotEmpty().WithMessage("Id is required.");
    RuleFor(command => command.CourseId).NotEmpty().WithMessage("CourseId is required.");
    Include(new ClassDetailsValidator(timeProvider));
  }
}
