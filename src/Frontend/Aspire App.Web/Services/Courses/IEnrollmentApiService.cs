namespace Aspire_App.Web.Services.Courses;

public interface IEnrollmentApiService
{
  Task EnrollToCourse(Guid courseId, Guid classId, CancellationToken cancellationToken = default);
  Task LeaveCourse(Guid courseId, Guid classId, CancellationToken cancellationToken = default);

  Task LeaveCourseByAdmin(string courseId, string classId, string studentId,
    CancellationToken cancellationToken = default);

  Task EnrollToCourseByAdmin(string courseId, string classId, string studentId,
    CancellationToken cancellationToken = default);
}
