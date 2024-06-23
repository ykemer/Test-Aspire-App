using ErrorOr;
using MediatR;

namespace Aspire_App.ApiService.Application.Enrollment.Command;

public record EnrollToCourseCommand(Guid CourseId, Guid StudentId)
    : IRequest<ErrorOr<Unit>>;