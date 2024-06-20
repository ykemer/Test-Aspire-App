using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Handlers;

public class CreateCourseHandler : IRequestHandler<CourseCreateCommand, CourseResponse>
{
    private readonly IMapper _mapper;
    private readonly ICourseRepository _courseRepository;

    public CreateCourseHandler(IMapper mapper, ICourseRepository studentRepository)
    {
        _mapper = mapper;
        _courseRepository = studentRepository;
    }

    public async Task<CourseResponse> Handle(CourseCreateCommand request, CancellationToken cancellationToken)
    {
        var studentToSave = _mapper.Map<Course>(request);
        await _courseRepository.AddAsync(studentToSave, cancellationToken);
        var output = _mapper.Map<CourseResponse>(studentToSave);
        return output;
    }
}