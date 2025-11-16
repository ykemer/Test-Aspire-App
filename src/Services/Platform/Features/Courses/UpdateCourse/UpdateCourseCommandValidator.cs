using Contracts.Classes.Requests;
using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseCommandValidator : Validator<UpdateCourseRequest>
{
  public UpdateCourseCommandValidator()
  {
    RuleFor(course => course.Name).NotEmpty().WithMessage("Name can not be empty");
    RuleFor(course => course.Description).NotEmpty().WithMessage("Description can not be empty");
  }
}
