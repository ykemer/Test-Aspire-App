using System.Security.Claims;

using Contracts.Enrollments.Requests;

using Platform.Common.Auth;

namespace Platform.Features.Enrollments;

/// <summary>
/// Decides WHO is being enrolled or unenrolled.
/// A student always acts for themselves: a student id in the request is ignored, so nobody can enroll someone else.
/// An administrator acts for another student and must say which one.
/// </summary>
public static class EnrollmentStudent
{
  public static readonly Error MissingStudentId =
    Error.Validation(nameof(ChangeCourseEnrollmentRequest.StudentId),
      "StudentId is required when an administrator changes a student's enrollment.");

  public static ErrorOr<Guid> Resolve(ClaimsPrincipal user, ChangeCourseEnrollmentRequest request)
  {
    if (!user.IsAdministrator())
    {
      return user.GetUserId();
    }

    return request.StudentId == Guid.Empty ? MissingStudentId : request.StudentId;
  }
}
