using Aspire_App.ApiService.Application.Students.Queries;
using Aspire_App.ApiService.Application.Students.Responses;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Pagination;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Handlers;

public class ListStudentsHandler : IRequestHandler<ListStudentsQuery, PagedList<StudentResponse>>
{
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public ListStudentsHandler(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
    }

    public Task<PagedList<StudentResponse>> Handle(ListStudentsQuery request, CancellationToken cancellationToken)
    {
        var output = _studentRepository
            .GetAllAsync(cancellationToken)
            .ProjectTo<StudentResponse>(_mapper.ConfigurationProvider);

        return Task.FromResult(PagedList<StudentResponse>.Create(output, request.Page, request.PageSize));
    }
}