using Contracts.Students.Responses;

using FastEndpoints;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

using StudentsGRPCClient;

namespace Platform.Features.Students.GetStudent;

public class GetStudentEndpoint : EndpointWithoutRequest<ErrorOr<StudentResponse>>
{
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;

  public GetStudentEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _studentsGrpcService = studentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/students/{StudentId}");
    Policies("RequireAdministratorRole");
    Options(x => x.RequireRateLimiting("fixed-per-user"));
  }

  public override async Task<ErrorOr<StudentResponse>> ExecuteAsync(CancellationToken ct)
  {
    var id = Route<Guid>("StudentId");

    var studentRequest =
      _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest { Id = id.ToString() });
    var
      studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    return studentResponse.Match<ErrorOr<StudentResponse>>(
      data => data.ToStudentResponse(),
      error => error);
  }
}
