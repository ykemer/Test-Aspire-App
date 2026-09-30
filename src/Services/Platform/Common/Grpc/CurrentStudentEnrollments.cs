using System.Security.Claims;

using EnrollmentsGRPCClient;

using Platform.Common.Auth;

namespace Platform.Common.Grpc;

/// <summary>
/// The classes and courses the signed-in student is enrolled in.
/// The Courses service uses the class ids to also show closed or full classes the student is already in.
/// </summary>
public sealed record StudentEnrollmentIds(IReadOnlyList<string> ClassIds, IReadOnlyList<string> CourseIds)
{
  public static readonly StudentEnrollmentIds None = new([], []);
}

public interface ICurrentStudentEnrollments
{
  /// <summary>
  /// Enrollments of the signed-in user. Administrators get <see cref="StudentEnrollmentIds.None"/>:
  /// they are not students and see everything anyway.
  /// </summary>
  Task<ErrorOr<StudentEnrollmentIds>> GetAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
}

public sealed class CurrentStudentEnrollments : ICurrentStudentEnrollments
{
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsClient;
  private readonly IGrpcCaller _grpc;

  public CurrentStudentEnrollments(GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsClient,
    IGrpcCaller grpc)
  {
    _enrollmentsClient = enrollmentsClient;
    _grpc = grpc;
  }

  public async Task<ErrorOr<StudentEnrollmentIds>> GetAsync(ClaimsPrincipal user,
    CancellationToken cancellationToken)
  {
    if (user.IsAdministrator())
    {
      return StudentEnrollmentIds.None;
    }

    var request = new GrpcGetStudentEnrollmentsRequest { StudentId = user.GetUserId().ToString() };
    var result = await _grpc.CallAsync(
      _enrollmentsClient.GetStudentEnrollmentsAsync(request, cancellationToken: cancellationToken));

    return result.Then(response => new StudentEnrollmentIds(
      response.Items.Select(enrollment => enrollment.ClassId).ToList(),
      response.Items.Select(enrollment => enrollment.CourseId).Distinct().ToList()));
  }
}
