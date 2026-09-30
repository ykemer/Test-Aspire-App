using Library.GRPC;

namespace Service.Enrollments.Features.Enrollments;

/// <summary>
/// Every error an enroll/unenroll operation can return.
/// One place, one naming style: "enrollments_service.enrollment.&lt;reason&gt;".
/// </summary>
public static class EnrollmentErrors
{
  public static Error ClassNotFound(Guid classId, Guid courseId) =>
    Error.NotFound("enrollments_service.enrollment.class_not_found",
      $"Class {classId} of course {courseId} was not found.");

  public static Error AlreadyEnrolled(Guid studentId, Guid classId) =>
    ConflictErrors.AlreadyExists("enrollments_service.enrollment.already_enrolled",
      $"Student {studentId} is already enrolled in class {classId}.");

  public static Error RegistrationClosed(Guid classId) =>
    ConflictErrors.StatePreventsAction("enrollments_service.enrollment.registration_closed",
      $"Registration for class {classId} is closed.");

  public static Error ClassFull(Guid classId) =>
    ConflictErrors.StatePreventsAction("enrollments_service.enrollment.class_full",
      $"Class {classId} has no free seats.");

  public static Error NotEnrolled(Guid studentId, Guid classId) =>
    Error.NotFound("enrollments_service.enrollment.not_enrolled",
      $"Student {studentId} is not enrolled in class {classId}.");

  public static Error ClassAlreadyStarted(Guid classId) =>
    ConflictErrors.StatePreventsAction("enrollments_service.enrollment.class_already_started",
      $"Class {classId} has already started, so students can no longer leave it.");
}
