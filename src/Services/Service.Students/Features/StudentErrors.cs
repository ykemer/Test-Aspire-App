using Library.GRPC;

namespace Service.Students.Features;

/// <summary>
/// Every error a student operation can return. One place, one naming style: "students_service.student.&lt;reason&gt;".
/// </summary>
public static class StudentErrors
{
  public static Error NotFound(Guid studentId) =>
    Error.NotFound("students_service.student.not_found",
      $"Student {studentId} was not found.");

  public static Error EmailAlreadyTaken(string email) =>
    ConflictErrors.AlreadyExists("students_service.student.email_already_taken",
      $"Another student already uses the email {email}.");

  public static Error HasEnrollments(Guid studentId) =>
    ConflictErrors.StatePreventsAction("students_service.student.has_enrollments",
      $"Student {studentId} cannot be deleted while enrolled in classes.");

  public static Error HasNoEnrollmentsToRemove(Guid studentId) =>
    ConflictErrors.StatePreventsAction("students_service.student.no_enrollments_to_remove",
      $"Student {studentId} has no enrollments to remove.");
}
