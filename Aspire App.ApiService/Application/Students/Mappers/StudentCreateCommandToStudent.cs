using Aspire_App.ApiService.Application.Students.Commands;
using Aspire_App.ApiService.Domain.Models;
using AutoMapper;

namespace Aspire_App.ApiService.Application.Students.Mappers
{
    public class StudentCreateCommandToStudent: Profile
    {
        public StudentCreateCommandToStudent()
        {
            CreateMap<StudentCreateCommand, Student>();
        }
    }
}
