using Contracts.Courses.Requests;
using FastEndpoints;
using FluentValidation;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseCommandValidator : Validator<DeleteCourseRequest>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(course => course.Id).NotEmpty().WithMessage("Name can not be empty");
    }
}