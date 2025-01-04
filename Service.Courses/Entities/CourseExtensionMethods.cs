using Contracts.Common;
using CoursesGRPC;

namespace Service.Courses.Entities;

public static class CourseExtensionMethods
{
    public static GrpcCourseResponse ToGrpcCourseResponse(this Course course)
    {
        return new GrpcCourseResponse
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            TotalStudents = course.EnrollmentsCount
        };
    }
    
    public static GrpcListCoursesResponse ToGrpcListCoursesResponse(this PagedList<Course> coursesResponse)
    {
        return new GrpcListCoursesResponse
        {
            CurrentPage = coursesResponse.CurrentPage,
            PageSize = coursesResponse.PageSize,
            TotalPages = coursesResponse.TotalPages,
            TotalCount = coursesResponse.TotalCount,
            Items = { coursesResponse.Items.Select(i => i.ToGrpcCourseResponse()) }
        };
    }
}