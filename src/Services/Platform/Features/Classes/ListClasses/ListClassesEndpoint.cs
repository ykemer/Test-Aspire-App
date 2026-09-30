using ClassesGRPCClient;

using Contracts.Classes.Responses;
using Contracts.Common;
using Contracts.Courses.Requests;

using FastEndpoints;

using Platform.Common.Auth;
using Platform.Common.Grpc;

namespace Platform.Features.Classes.ListClasses;

/// <summary>
/// Returns one page of the classes of a course. Students see open classes and the classes they are in,
/// each marked with "is the user enrolled". Administrators see every class.
/// </summary>
public class ListClassesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<ClassListItemResponse>>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesClient;
  private readonly ICurrentStudentEnrollments _currentStudentEnrollments;
  private readonly IGrpcCaller _grpc;

  public ListClassesEndpoint(GrpcClassService.GrpcClassServiceClient classesClient,
    ICurrentStudentEnrollments currentStudentEnrollments, IGrpcCaller grpc)
  {
    _classesClient = classesClient;
    _currentStudentEnrollments = currentStudentEnrollments;
    _grpc = grpc;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId:guid}/classes");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Classes"));
  }

  public override async Task<ErrorOr<PagedList<ClassListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var enrollments = await _currentStudentEnrollments.GetAsync(User, ct);
    if (enrollments.IsError)
    {
      return enrollments.Errors;
    }

    var enrolledClassIds = enrollments.Value.ClassIds;
    var request = query.ToGrpcListClassRequest(Route<Guid>("CourseId"), enrolledClassIds, User.IsAdministrator());

    var result = await _grpc.CallAsync(_classesClient.ListClassesAsync(request, cancellationToken: ct));
    return result.Then(classes => classes.ToClassListItemResponse(enrolledClassIds));
  }
}
