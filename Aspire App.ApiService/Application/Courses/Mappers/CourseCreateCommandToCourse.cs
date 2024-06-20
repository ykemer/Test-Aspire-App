using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Domain.Models;
using AutoMapper;

namespace Aspire_App.ApiService.Application.Courses.Mappers;

public class CourseCreateCommandToCourse : Profile
{
    public CourseCreateCommandToCourse()
    {
        CreateMap<CourseCreateCommand, Course>();
    }
}