using Contracts.Common;
using Contracts.Students.Requests;
using Contracts.Students.Responses;

using FastEndpoints;

using Platform.Common.Middleware.Grpc;

using StudentsGRPCClient;

namespace Platform.Features.Students.ListStudents;

public class ListStudentsEndpoint : Endpoint<ListStudentsRequest, ErrorOr<PagedList<StudentResponse>>>
{
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;

  public ListStudentsEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _studentsGrpcService = studentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/students");
    Policies("RequireAdministratorRole");
    Options(x => x.RequireRateLimiting("fixed-per-user"));
    Description(x => x.WithTags("Students"));
  }

  public override async Task<ErrorOr<PagedList<StudentResponse>>> ExecuteAsync(ListStudentsRequest query,
    CancellationToken ct)
  {
    var studentRequest =
      _studentsGrpcService.ListStudentsAsync(query.ToGrpcListStudentsRequest());
    var studentResponse =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    return studentResponse.Match<ErrorOr<PagedList<StudentResponse>>>(
      data => data.ToStudentListResponse(),
      error => error);
  }
}
