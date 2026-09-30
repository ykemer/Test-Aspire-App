using FluentValidation;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesQueryValidator : AbstractValidator<ListCoursesQuery>
{
  public ListCoursesQueryValidator()
  {
    RuleFor(query => query.PageNumber).GreaterThan(0).WithMessage("Page must be 1 or more.");
    RuleFor(query => query.PageSize).GreaterThan(0).WithMessage("Page size must be 1 or more.");
    RuleFor(query => query.Query)
      .MaximumLength(CourseLimits.SearchQueryMaxLength)
      .WithMessage($"Search text must not exceed {CourseLimits.SearchQueryMaxLength} characters.");
  }
}
