using Contracts.Classes.Requests;

using FastEndpoints;

namespace Platform.Features.Classes.UpdateClass;

public class UpdateClassCommandValidator : Validator<UpdateClassRequest>
{
  public UpdateClassCommandValidator() : this(TimeProvider.System)
  {
  }

  public UpdateClassCommandValidator(TimeProvider timeProvider) => Include(new ClassScheduleRules(timeProvider));
}
