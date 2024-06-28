using Contracts.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries;

public record GetStudentQuery(Guid Id) : IRequest<StudentResponse?>;