using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseCommandValidator : Validator<CreateCourseRequest>
{
  public CreateCourseCommandValidator()
  {
    RuleFor(course => course.Name).NotEmpty().WithMessage("Name can not be empty");
    RuleFor(course => course.Description).NotEmpty().WithMessage("Description can not be empty");
  }
}
