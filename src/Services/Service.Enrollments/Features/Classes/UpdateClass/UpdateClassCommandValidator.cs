using FluentValidation;

namespace Service.Enrollments.Features.Classes.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
  public UpdateClassCommandValidator() => Include(new ClassDetailsValidator());
}
