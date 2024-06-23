using Aspire_App.ApiService.Application.Courses.Command;
using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using ErrorOr;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Handlers;

public class CreateCourseHandler : IRequestHandler<CourseCreateCommand, ErrorOr<CourseResponse>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CreateCourseHandler(IMapper mapper, ICourseRepository studentRepository)
    {
        _mapper = mapper;
        _courseRepository = studentRepository;
    }

    public async Task<ErrorOr<CourseResponse>> Handle(CourseCreateCommand request, CancellationToken cancellationToken)
    {
        var course = _courseRepository.GetAllAsync(cancellationToken).FirstOrDefault(i => i.Name == request.Name);
        if (course != null) return Error.Conflict(description: "Course already exists");

        var courseToSave = _mapper.Map<Course>(request);
        await _courseRepository.AddAsync(courseToSave, cancellationToken);
        return _mapper.Map<CourseResponse>(courseToSave);
    }
}