using Contracts.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Commands;

public record StudentCreateCommand(Guid Id, string FirstName, string LastName, string Email, DateTime DateOfBirth)
    : IRequest<StudentResponse>;