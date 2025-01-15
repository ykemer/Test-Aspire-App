using Contracts.Courses.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class StudentEnrollRequestValidator : Validator<ChangeCourseEnrollmentRequest>
{
  public StudentEnrollRequestValidator() =>
    RuleFor(course => course.CourseId).NotEmpty().WithMessage("Course id can not be empty");
}
