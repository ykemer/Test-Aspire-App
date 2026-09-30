using Contracts.Courses.Requests;

using FastEndpoints;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseCommandValidator : Validator<UpdateCourseRequest>
{
  public UpdateCourseCommandValidator() => Include(new CourseDetailsRules());
}
