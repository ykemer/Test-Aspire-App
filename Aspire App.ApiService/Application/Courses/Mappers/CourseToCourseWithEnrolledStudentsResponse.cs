using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Domain.Models;
using AutoMapper;

namespace Aspire_App.ApiService.Application.Courses.Mappers;

public class CourseToCourseWithEnrolledStudentsResponse : Profile
{
    public CourseToCourseWithEnrolledStudentsResponse()
    {
        CreateMap<Course, CourseWithEnrolledStudentsResponse>();
    }
}