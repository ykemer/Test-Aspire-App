using Library.GRPC;

namespace Service.Enrollments.Features.Classes;

/// <summary>
/// Every error a class operation can return. One place, one naming style: "enrollments_service.class.&lt;reason&gt;".
/// </summary>
public static class ClassErrors
{
  public static Error NotFound(Guid classId) =>
    Error.NotFound("enrollments_service.class.not_found",
      $"Class {classId} was not found.");

  public static Error HasEnrollments(Guid classId) =>
    ConflictErrors.StatePreventsAction("enrollments_service.class.has_enrollments",
      $"Class {classId} cannot be deleted because students are enrolled in it.");

  public static Error CourseHasEnrollments(Guid courseId) =>
    ConflictErrors.StatePreventsAction("enrollments_service.class.course_has_enrollments",
      $"Classes of course {courseId} cannot be deleted because students are enrolled in them.");
}
