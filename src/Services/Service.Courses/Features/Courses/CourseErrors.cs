using Library.GRPC;

namespace Service.Courses.Features.Courses;

/// <summary>
/// Every error a course operation can return. One place, one naming style: "courses_service.course.&lt;reason&gt;".
/// </summary>
public static class CourseErrors
{
  public static Error NotFound(Guid courseId) =>
    Error.NotFound("courses_service.course.not_found",
      $"Course {courseId} was not found.");

  public static Error NameAlreadyTaken(string name) =>
    ConflictErrors.AlreadyExists("courses_service.course.name_already_taken",
      $"A course named '{name}' already exists.");

  public static Error HasEnrolledStudents(Guid courseId) =>
    ConflictErrors.StatePreventsAction("courses_service.course.has_enrolled_students",
      $"Course {courseId} cannot be deleted because students are enrolled in it.");

  public static Error ModifiedConcurrently(Guid courseId) =>
    ConflictErrors.ConcurrentUpdate("courses_service.course.modified_concurrently",
      $"Course {courseId} was changed by someone else. Please reload and try again.");

  public static Error HasNoStudentsToRemove(Guid courseId) =>
    ConflictErrors.StatePreventsAction("courses_service.course.no_students_to_remove",
      $"Course {courseId} has no enrolled students to remove.");
}
