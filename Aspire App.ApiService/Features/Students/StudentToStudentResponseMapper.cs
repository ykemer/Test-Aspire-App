using AutoMapper;
using Contracts.Students.Responses;

namespace Aspire_App.ApiService.Features.Students;

public class StudentToStudentResponseMapper : Profile
{
    public StudentToStudentResponseMapper()
    {
        CreateMap<Student, StudentResponse>();
    }
}