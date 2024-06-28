using Aspire_App.ApiService.Domain.Models;
using AutoMapper;
using Contracts.Students.Responses;

namespace Aspire_App.ApiService.Application.Students.Mappers;

public class StudentToStudentResponse : Profile
{
    public StudentToStudentResponse()
    {
        CreateMap<Student, StudentResponse>();
    }
}