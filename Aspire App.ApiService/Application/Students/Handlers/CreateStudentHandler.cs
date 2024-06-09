using Aspire_App.ApiService.Application.Students.Commands;
using Aspire_App.ApiService.Application.Students.Responses;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using MediatR;

namespace Aspire_App.ApiService.Application.Students.Handlers;

public class CreateStudentHandler : IRequestHandler<StudentCreateCommand, StudentResponse>
{
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;

    public CreateStudentHandler(IMapper mapper, IStudentRepository studentRepository)
    {
        _mapper = mapper;
        _studentRepository = studentRepository;
    }

    public async Task<StudentResponse> Handle(StudentCreateCommand request, CancellationToken cancellationToken)
    {
        var studentToSave = _mapper.Map<Student>(request);
        await _studentRepository.AddAsync(studentToSave, cancellationToken);
        var output = _mapper.Map<StudentResponse>(studentToSave);
        return output;
    }
}