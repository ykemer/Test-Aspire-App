using Contracts.Students.Responses;

using FastEndpoints;

using Grpc.Core;

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
  }

  public override async Task<ErrorOr<StudentResponse>> ExecuteAsync(CancellationToken ct)
  {
    Guid id = Route<Guid>("StudentId");

    AsyncUnaryCall<GrpcStudentResponse>? studentRequest =
      _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest { Id = id.ToString() });
    ErrorOr<GrpcStudentResponse>
      studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    return studentResponse.Match<ErrorOr<StudentResponse>>(
      data => data.ToStudentResponse(),
      error => error);
  }
}
