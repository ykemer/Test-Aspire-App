using ClassesGRPCClient;

using Contracts.Classes.Responses;
using Contracts.Courses.Responses;

using FastEndpoints;

using Platform.Common.Auth;
using Platform.Common.Grpc;

namespace Platform.Features.Classes.GetClass;

/// <summary>
/// Returns one class. Students see a class only if it is open for registration,
/// or if they are already enrolled in it. Administrators see every class.
/// </summary>
public class GetClassEndpoint : EndpointWithoutRequest<ErrorOr<ClassResponse>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesClient;
  private readonly ICurrentStudentEnrollments _currentStudentEnrollments;
  private readonly IGrpcCaller _grpc;

  public GetClassEndpoint(GrpcClassService.GrpcClassServiceClient classesClient,
    ICurrentStudentEnrollments currentStudentEnrollments, IGrpcCaller grpc)
  {
    _classesClient = classesClient;
    _currentStudentEnrollments = currentStudentEnrollments;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId:guid}/classes/{ClassId:guid}");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<ClassResponse>> ExecuteAsync(CancellationToken ct)
  {
    var enrollments = await _currentStudentEnrollments.GetAsync(User, ct);
    if (enrollments.IsError)
    {
      return enrollments.Errors;
    }

    var request = new GrpcGetClassRequest
    {
      Id = Route<Guid>("ClassId").ToString(),
      CourseId = Route<Guid>("CourseId").ToString(),
      EnrolledClasses = { enrollments.Value.ClassIds },
      ShowAll = User.IsAdministrator()
    };

    var result = await _grpc.CallAsync(_classesClient.GetClassAsync(request, cancellationToken: ct));
    return result.Then(courseClass => courseClass.ToClassResponse());
  }
}
