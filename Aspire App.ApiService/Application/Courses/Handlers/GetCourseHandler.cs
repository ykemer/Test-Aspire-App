using Aspire_App.ApiService.Application.Courses.Queries;
using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Application.Students.Responses;
using Aspire_App.ApiService.Domain.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Aspire_App.ApiService.Application.Courses.Handlers;

public class GetCourseHandler : IRequestHandler<GetCourseQuery, CourseWithEnrolledStudentsResponse?>
{
    private readonly IMapper _mapper;
    private readonly ICourseRepository _courseRepository;

    public GetCourseHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<CourseWithEnrolledStudentsResponse?> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var result = await _courseRepository
            .GetAllAsync(cancellationToken)
            .Include(i => i.StudentEnrollments)
            .ThenInclude(x => x.Student)
            .Where(i => i.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        
        if(result == null) return null;
        

        var course =  _mapper.Map<CourseWithEnrolledStudentsResponse>(result);
        var students = result.StudentEnrollments.Select(x => _mapper.Map<StudentResponse>(x.Student)).ToList();
        course.EnrolledStudents = students;
        return course;   
    }
}