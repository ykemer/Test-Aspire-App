using Aspire_App.ApiService.Application.Courses.Command;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Validators.Courses;

public class CourseCreateCommandValidator : Validator<CourseCreateCommand>
{
    public CourseCreateCommandValidator()
    {
        RuleFor(course => course.Name).NotEmpty().WithMessage("Name can not be empty");
        RuleFor(course => course.Description).NotEmpty().WithMessage("Description can not be empty");
    }
}