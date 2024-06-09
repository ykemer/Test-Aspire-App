using Aspire_App.ApiService.Application.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Commands
{
    public record StudentCreateCommand(string FirstName, string LastName, string Email, DateTime DateOfBirth)
        : IRequest<StudentResponse>;
}
