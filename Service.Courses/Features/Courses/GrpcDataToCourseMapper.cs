using CoursesGRPC;
using Service.Courses.Entities;
using Service.Courses.Features.Courses.CreateCourse;

namespace Service.Courses.Features.Courses;

public static class GrpcDataToCourseMapper
{
    public static CreateCourseCommand MapGrpcCreateCourseRequestToCreateCourseCommand(GrpcCreateCourseRequest request)
    {
        return new CreateCourseCommand(request.Name, request.Description);
    }
    
    public static GrpcCourseResponse MapCourseToGrpcCourseResponse(Course course)
    {
        return new GrpcCourseResponse
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            TotalStudents = course.EnrollmentsCount
        };
    }
}