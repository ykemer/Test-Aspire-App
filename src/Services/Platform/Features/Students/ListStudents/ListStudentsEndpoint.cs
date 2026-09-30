using Contracts.Common;
using Contracts.Students.Requests;
using Contracts.Students.Responses;

using FastEndpoints;

using Platform.Common.Grpc;

using StudentsGRPCClient;

namespace Platform.Features.Students.ListStudents;

/// <summary>
/// Administrators: returns one page of students.
/// </summary>
public class ListStudentsEndpoint : Endpoint<ListStudentsRequest, ErrorOr<PagedList<StudentResponse>>>
{
  private readonly IGrpcCaller _grpc;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsClient;

  public ListStudentsEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsClient, IGrpcCaller grpc)
  {
    _studentsClient = studentsClient;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/students");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Students"));
  }

  public override async Task<ErrorOr<PagedList<StudentResponse>>> ExecuteAsync(ListStudentsRequest query,
    CancellationToken ct)
  {
    var result = await _grpc.CallAsync(
      _studentsClient.ListStudentsAsync(query.ToGrpcListStudentsRequest(), cancellationToken: ct));
    return result.Then(students => students.ToStudentListResponse());
  }
}
