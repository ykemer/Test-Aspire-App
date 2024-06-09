using Aspire_App.ApiService.Application.Students.Responses;
using Aspire_App.ApiService.Domain.Models;
using AutoMapper;

namespace Aspire_App.ApiService.Application.Students.Mappers;

public class StudentToStudentResponse : Profile
{
    public StudentToStudentResponse()
    {
        CreateMap<Student, StudentResponse>();
    }
}