using AutoMapper;
using Contracts.Courses.Requests;

namespace Aspire_App.ApiService.Features.Courses;

public class CourseCreateCommandToCourseMapper : Profile
{
    public CourseCreateCommandToCourseMapper()
    {
        CreateMap<CreateCourseRequest, Course>();
    }
}