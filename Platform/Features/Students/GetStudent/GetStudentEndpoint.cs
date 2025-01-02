using Contracts.Students.Responses;
using FastEndpoints;
using Platform.Features.Enrollments.EnrollToCourse;
using Platform.Services.Middleware;
using StudentsGRPCClient;

namespace Platform.Features.Students.GetStudent;

public class GetStudentEndpoint : EndpointWithoutRequest<ErrorOr<StudentResponse>>
{
    private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
    private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

    public GetStudentEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService, IGrpcRequestMiddleware grpcRequestMiddleware)
    {
        _studentsGrpcService = studentsGrpcService;
        _grpcRequestMiddleware = grpcRequestMiddleware;
    }

    public override void Configure()
    {
        Get("/api/students/{StudentId}");
        Policies("RequireAdministratorRole");
    }

    public override async Task<ErrorOr<StudentResponse>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("StudentId");

        var studentRequest = _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest
        {
            Id = id.ToString()
        });
        var studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
        return studentResponse.Match<ErrorOr<StudentResponse>>(
            data => GrpcStudentToStudentResponseMapper.MapToStudentResponse(data),
            error => error);
    }
}