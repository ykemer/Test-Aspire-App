using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Application.Students.Responses;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Application.Students.Handlers;

public class ListStudentsHandler : IRequestHandler<ListStudentsQuery, List<StudentResponse>>
{
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public ListStudentsHandler(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public async Task<List<StudentResponse>> Handle(ListStudentsQuery request, CancellationToken cancellationToken)
    {
        return await _studentRepository
            .GetAllAsync(cancellationToken)
            .ProjectTo<StudentResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}