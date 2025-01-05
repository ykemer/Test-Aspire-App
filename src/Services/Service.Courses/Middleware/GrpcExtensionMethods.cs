using CoursesGRPC;
using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;

namespace Service.Courses.Middleware;

public static class GrpcExtensionMethods
{
    public static CreateCourseCommand ToCreateCourseCommand(this GrpcCreateCourseRequest request)
    {
        return new CreateCourseCommand(request.Name, request.Description);
    }
    
    public static UpdateCourseCommand ToUpdateCourseCommand(this GrpcUpdateCourseRequest request)
    {
        return new UpdateCourseCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };
    }
    
    public static DeleteCourseCommand ToDeleteCourseCommand(this GrpcDeleteCourseRequest request)
    {
        return new DeleteCourseCommand(request.Id);
    }
    
    public static GetCourseQuery ToGetCourseQuery(this GrpcGetCourseRequest request)
    {
        return new GetCourseQuery(request.Id);
    }
    
    public static ListCoursesRequest ToListCoursesRequest(this GrpcListCoursesRequest request)
    {
        return new ListCoursesRequest
        {
            PageSize = request.PageSize,
            PageNumber = request.Page,
            Query = request.Query
        };
    }
}