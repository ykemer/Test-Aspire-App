using Contracts.Common;
using Contracts.Students.Requests;
using Contracts.Students.Responses;
using FastEndpoints;
using Platform.Features.Enrollments.EnrollToCourse;
using Platform.Services.Middleware;
using StudentsGRPCClient;

namespace Platform.Features.Students.ListStudents;

public class ListStudentsEndpoint : Endpoint<ListStudentsRequest, ErrorOr<PagedList<StudentResponse>>>
{
    private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
    private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

    public ListStudentsEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService, IGrpcRequestMiddleware grpcRequestMiddleware)
    {
        _studentsGrpcService = studentsGrpcService;
        _grpcRequestMiddleware = grpcRequestMiddleware;
    }

    public override void Configure()
    {
        Get("/api/students/list");
        Policies("RequireAdministratorRole");
    }

    public override async Task<ErrorOr<PagedList<StudentResponse>>> ExecuteAsync(ListStudentsRequest query,
        CancellationToken ct)
    {
        var studentRequest = _studentsGrpcService.ListStudentsAsync(new GrpcListStudentsRequest
        {
            Page = query.PageNumber,
            PageSize = query.PageSize
        });
        var studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
        return studentResponse.Match<ErrorOr<PagedList<StudentResponse>>>(
            data => new PagedList<StudentResponse>
            {
                Items = data.Items.Select(GrpcStudentToStudentResponseMapper.MapToStudentResponse).ToList(),
                CurrentPage = data.CurrentPage,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount
            },
            error => error);
    }
}