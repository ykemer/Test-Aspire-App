using Contracts.Courses.Requests.Courses;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

namespace Platform.Features.Courses.CreateCourse;

public static class CreateCourseMapper
{
  public static GrpcCreateCourseRequest ToGrpcCreateCourseRequest(this CreateCourseRequest createCourseCommand) =>
    new() { Name = createCourseCommand.Name, Description = createCourseCommand.Description };

  public static CourseResponse ToCourseResponse(this GrpcCourseResponse course) =>
    new()
    {
      Id = Guid.Parse(course.Id),
      Description = course.Description,
      Name = course.Name,
      TotalStudents = course.TotalStudents
    };
}
