using CoursesGRPC;

using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Service.Courses.Middleware;

public static class GrpcExtensionMethods
{
  public static CreateCourseCommand MapToCreateCourseCommand(this GrpcCreateCourseRequest request) =>
    new(request.Name, request.Description);

  public static UpdateCourseCommand MapToUpdateCourseCommand(this GrpcUpdateCourseRequest request) =>
    new() { Id = request.Id, Name = request.Name, Description = request.Description };

  public static DeleteCourseCommand MapToDeleteCourseCommand(this GrpcDeleteCourseRequest request) => new(request.Id);

  public static GetCourseQuery ToGetCourseQuery(this GrpcGetCourseRequest request) => new(request.Id);

  public static ListCoursesRequest MapToListCoursesRequest(this GrpcListCoursesRequest request) =>
    new() { PageSize = request.PageSize, PageNumber = request.Page, Query = request.Query };
}
