namespace Aspire_App.ApiService.Application.Students.Mappers;

using Aspire_App.ApiService.Domain.Models;
using AutoMapper;


public class StudentToStudentResponse: Profile
{
    public StudentToStudentResponse()
    {
        CreateMap<Student, Responses.StudentResponse>();
    }
}