using ClassesGRPCClient;

using Contracts.Enrollments.Responses;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Platform.Common.Grpc;

namespace Platform.Features.Enrollments.GetClassEnrollments;

/// <summary>
/// Administrators: lists the students enrolled in one class. Returns 404 if the class does not exist.
/// </summary>
public class GetClassEnrollmentsEndpoint : EndpointWithoutRequest<ErrorOr<List<EnrollmentResponse>>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesClient;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsClient;
  private readonly IGrpcCaller _grpc;

  public GetClassEnrollmentsEndpoint(GrpcClassService.GrpcClassServiceClient classesClient,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsClient, IGrpcCaller grpc)
  {
    _classesClient = classesClient;
    _enrollmentsClient = enrollmentsClient;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId:guid}/classes/{ClassId:guid}/enrollments");
    Policies(Common.Auth.Policies.Administrators);
    Description(x => x.WithTags("Enrollments"));
  }

  public override async Task<ErrorOr<List<EnrollmentResponse>>> ExecuteAsync(CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId").ToString();
    var classId = Route<Guid>("ClassId").ToString();

    var courseClass = await _grpc.CallAsync(_classesClient.GetClassAsync(
      new GrpcGetClassRequest { Id = classId, CourseId = courseId, ShowAll = true }, cancellationToken: ct));
    if (courseClass.IsError)
    {
      return courseClass.Errors;
    }

    var enrollments = await _grpc.CallAsync(_enrollmentsClient.GetClassEnrollmentsAsync(
      new GrpcGetClassEnrollmentsRequest { CourseId = courseId, ClassId = classId }, cancellationToken: ct));
    return enrollments.Then(response => response.ToEnrollmentResponseList());
  }
}
