using FluentValidation;

namespace Service.Enrollments.Features.Classes.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
  public CreateClassCommandValidator() => Include(new ClassDetailsValidator());
}
