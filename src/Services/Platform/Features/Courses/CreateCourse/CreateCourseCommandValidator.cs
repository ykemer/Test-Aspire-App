using Contracts.Courses.Requests;

using FastEndpoints;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseCommandValidator : Validator<CreateCourseRequest>
{
  public CreateCourseCommandValidator() => Include(new CourseDetailsRules());
}
