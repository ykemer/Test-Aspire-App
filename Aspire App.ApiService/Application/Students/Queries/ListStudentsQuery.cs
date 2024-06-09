using Aspire_App.ApiService.Application.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries;

public record ListStudentsQuery : IRequest<List<StudentResponse>>;