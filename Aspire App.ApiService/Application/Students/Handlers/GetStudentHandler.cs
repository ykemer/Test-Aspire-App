using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using Contracts.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Handlers;

public class GetStudentHandler : IRequestHandler<GetStudentQuery, StudentResponse?>
{
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public GetStudentHandler(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<StudentResponse?> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var result = await _studentRepository
            .GetAsync(request.Id, cancellationToken);

        return result == null ? null : _mapper.Map<StudentResponse>(result);
    }
}