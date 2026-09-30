using Library.GRPC;

namespace Service.Courses.Features.Classes;

/// <summary>
/// Every error a class operation can return. One place, one naming style: "courses_service.class.&lt;reason&gt;".
/// </summary>
public static class ClassErrors
{
  public static Error NotFound(Guid classId, Guid courseId) =>
    Error.NotFound("courses_service.class.not_found",
      $"Class {classId} of course {courseId} was not found.");

  public static Error HasEnrolledStudents(Guid classId) =>
    ConflictErrors.StatePreventsAction("courses_service.class.has_enrolled_students",
      $"Class {classId} cannot be deleted because students are enrolled in it.");

  public static Error MaxStudentsBelowEnrolledCount(Guid classId) =>
    ConflictErrors.StatePreventsAction("courses_service.class.max_students_below_enrolled_count",
      $"Class {classId} already has more students than the new maximum.");

  public static Error ModifiedConcurrently(Guid classId) =>
    ConflictErrors.ConcurrentUpdate("courses_service.class.modified_concurrently",
      $"Class {classId} was changed by someone else. Please reload and try again.");

  public static Error IsFull(Guid classId) =>
    ConflictErrors.StatePreventsAction("courses_service.class.is_full",
      $"Class {classId} has no free seats.");

  public static Error HasNoStudentsToRemove(Guid classId) =>
    ConflictErrors.StatePreventsAction("courses_service.class.no_students_to_remove",
      $"Class {classId} has no enrolled students to remove.");
}
