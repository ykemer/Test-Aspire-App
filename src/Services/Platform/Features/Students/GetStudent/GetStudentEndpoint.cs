using Contracts.Students.Responses;

using FastEndpoints;

using Platform.Common.Grpc;

using StudentsGRPCClient;

namespace Platform.Features.Students.GetStudent;

/// <summary>
/// Administrators: returns one student.
/// </summary>
public class GetStudentEndpoint : EndpointWithoutRequest<ErrorOr<StudentResponse>>
{
  private readonly IGrpcCaller _grpc;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsClient;

  public GetStudentEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsClient, IGrpcCaller grpc)
  {
    _studentsClient = studentsClient;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/students/{StudentId:guid}");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Students"));
  }

  public override async Task<ErrorOr<StudentResponse>> ExecuteAsync(CancellationToken ct)
  {
    var request = new GrpcGetStudentByIdRequest { Id = Route<Guid>("StudentId").ToString() };
    var result = await _grpc.CallAsync(_studentsClient.GetStudentByIdAsync(request, cancellationToken: ct));
    return result.Then(student => student.ToStudentResponse());
  }
}
