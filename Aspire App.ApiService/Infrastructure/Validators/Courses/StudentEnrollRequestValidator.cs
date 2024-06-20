using Aspire_App.ApiService.Infrastructure.Endpoints.Courses.Requrests.Courses;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Validators.Courses;

public class StudentEnrollRequestValidator : Validator<StudentChangeEnrollRequest>
{
    public StudentEnrollRequestValidator()
    {
        RuleFor(course => course.CourseId).NotEmpty().WithMessage("Course id can not be empty");
    }
}