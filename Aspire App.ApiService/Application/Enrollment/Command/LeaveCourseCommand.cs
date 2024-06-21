using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Command;

public record LeaveCourseCommand(Guid CourseId, Guid StudentId)
    : IRequest;