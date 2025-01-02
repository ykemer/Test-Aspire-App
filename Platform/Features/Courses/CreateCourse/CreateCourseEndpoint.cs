using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using CoursesGRPCClient;
using FastEndpoints;
using Platform.Services.Middleware;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseEndpoint : Endpoint<CreateCourseRequest,
    ErrorOr<CourseResponse>>
{
    private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
    private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

    public CreateCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
        IGrpcRequestMiddleware grpcRequestMiddleware)
    {
        _coursesGrpcService = coursesGrpcService;
        _grpcRequestMiddleware = grpcRequestMiddleware;
    }

    public override void Configure()
    {
        Post("/api/courses/create");
        Policies("RequireAdministratorRole");
    }

    public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(CreateCourseRequest createCourseCommand,
        CancellationToken ct)
    {
        var request = _coursesGrpcService.CreateCourseAsync(new GrpcCreateCourseRequest
        {
            Name = createCourseCommand.Name,
            Description = createCourseCommand.Description
        }, cancellationToken: ct);

        var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
        return result.Match<ErrorOr<CourseResponse>>(
            data => new CourseResponse
            {
                Id = Guid.Parse(data.Id),
                Description = data.Description,
                Name = data.Name,
            },
            error => error);
    }
}