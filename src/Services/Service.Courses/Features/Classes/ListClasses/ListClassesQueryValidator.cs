using FluentValidation;

namespace Service.Courses.Features.Classes.ListClasses;

public class ListClassesQueryValidator : AbstractValidator<ListClassesQuery>
{
  public ListClassesQueryValidator()
  {
    RuleFor(query => query.CourseId).NotEmpty().WithMessage("CourseId is required.");
    RuleFor(query => query.PageNumber).GreaterThan(0).WithMessage("Page must be 1 or more.");
    RuleFor(query => query.PageSize).GreaterThan(0).WithMessage("Page size must be 1 or more.");
  }
}
