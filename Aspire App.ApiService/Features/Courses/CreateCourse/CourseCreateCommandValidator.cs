using Contracts.Courses.Requests;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Features.Courses.CreateCourse;

public class CourseCreateCommandValidator : Validator<CreateCourseRequest>
{
    public CourseCreateCommandValidator()
    {
        RuleFor(course => course.Name).NotEmpty().WithMessage("Name can not be empty");
        RuleFor(course => course.Description).NotEmpty().WithMessage("Description can not be empty");
    }
}