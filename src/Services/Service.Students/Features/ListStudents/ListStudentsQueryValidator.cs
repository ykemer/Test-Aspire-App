using FluentValidation;

namespace Service.Students.Features.ListStudents;

public class ListStudentsQueryValidator : AbstractValidator<ListStudentsQuery>
{
  public ListStudentsQueryValidator()
  {
    RuleFor(query => query.PageNumber).GreaterThan(0).WithMessage("Page must be 1 or more.");
    RuleFor(query => query.PageSize).GreaterThan(0).WithMessage("Page size must be 1 or more.");
  }
}
