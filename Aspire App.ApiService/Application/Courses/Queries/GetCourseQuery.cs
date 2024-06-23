using Aspire_App.ApiService.Application.Courses.Responses;
using ErrorOr;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Queries;

public record GetCourseQuery(Guid Id) : IRequest<ErrorOr<CourseWithEnrolledStudentsResponse>>;