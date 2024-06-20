using Aspire_App.ApiService.Application.Courses.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Queries;

public record GetCourseQuery(Guid Id) : IRequest<CourseWithEnrolledStudentsResponse?>;