using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Domain.Models;
using AutoMapper;

namespace Aspire_App.ApiService.Application.Courses.Mappers;

public class CourseToCourseResponse : Profile
{
    public CourseToCourseResponse()
    {
        CreateMap<Course, CourseResponse>();
    }
}