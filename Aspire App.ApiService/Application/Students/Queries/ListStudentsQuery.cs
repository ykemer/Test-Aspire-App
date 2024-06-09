using Aspire_App.ApiService.Application.Students.Responses;
using ErrorOr;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Queries
{
    public record ListStudentsQuery : IRequest<List<StudentResponse>>;

}
