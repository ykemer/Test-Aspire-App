using MediatR;

namespace Aspire_App.ApiService.Application.Courses.Command;

public record EnrollToCourseCommand(Guid CourseId, Guid StudentId)
    : IRequest;