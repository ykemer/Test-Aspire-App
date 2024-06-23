using Aspire_App.ApiService.Application.Courses.Responses;
using ErrorOr;
using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Command;

public record CourseCreateCommand(string Name, string Description)
    : IRequest<ErrorOr<CourseResponse>>;