using Aspire_App.ApiService.Application.Courses.Responses;
using Aspire_App.ApiService.Application.Students.Responses;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Command;

public record CourseCreateCommand(string Name, string Description)
    : IRequest<CourseResponse>;