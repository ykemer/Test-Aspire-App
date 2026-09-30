using Contracts.Classes.Requests;

using FastEndpoints;

namespace Platform.Features.Classes.CreateClass;

public class CreateClassCommandValidator : Validator<CreateClassRequest>
{
  public CreateClassCommandValidator() : this(TimeProvider.System)
  {
  }

  public CreateClassCommandValidator(TimeProvider timeProvider) => Include(new ClassScheduleRules(timeProvider));
}
