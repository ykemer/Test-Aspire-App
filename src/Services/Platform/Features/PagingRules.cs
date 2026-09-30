using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Students.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features;

/// <summary>
/// Paging rules for every list request. (The page size is already capped at 50 by PagedQuery itself.)
/// </summary>
[DontRegister] // a building block for the list validators below, not a validator of its own
public class PagingRules : AbstractValidator<PagedQuery>
{
  public const int SearchTextMaxLength = 200;

  public PagingRules()
  {
    RuleFor(query => query.PageNumber).GreaterThan(0).WithMessage("Page must be 1 or more.");
    RuleFor(query => query.PageSize).GreaterThan(0).WithMessage("Page size must be 1 or more.");
    RuleFor(query => query.Query)
      .MaximumLength(SearchTextMaxLength)
      .WithMessage($"Search text must not exceed {SearchTextMaxLength} characters.");
  }
}

/// <summary>Used by "list courses" and "list classes" (both use <see cref="ListCoursesRequest"/>).</summary>
public class ListCoursesRequestValidator : Validator<ListCoursesRequest>
{
  public ListCoursesRequestValidator() => Include(new PagingRules());
}

public class ListStudentsRequestValidator : Validator<ListStudentsRequest>
{
  public ListStudentsRequestValidator() => Include(new PagingRules());
}
