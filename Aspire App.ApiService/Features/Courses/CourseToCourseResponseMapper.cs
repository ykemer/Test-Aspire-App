using AutoMapper;
using Contracts.Courses.Responses;
using Contracts.Students.Responses;

namespace Aspire_App.ApiService.Features.Courses;

public class CourseToCourseResponseMapper : Profile
{
    public CourseToCourseResponseMapper()
    {
        CreateMap<Course, CourseResponse>()
            .ForMember(dest => dest.Enrolled, src => src.MapFrom(i => i.StudentEnrollments.Any()))
            .ForMember(dest => dest.EnrolledStudents, src => src.MapFrom(i => i.StudentEnrollments.Select(se =>
                new StudentResponse
                {
                    Id = se.Student.Id,
                    Email = se.Student.Email,
                    FirstName = se.Student.FirstName,
                    LastName = se.Student.LastName,
                    DateOfBirth = se.Student.DateOfBirth
                })));
    }
}