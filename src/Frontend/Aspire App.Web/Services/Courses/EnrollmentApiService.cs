using Contracts.Courses.Requests.Enrollments;

namespace Aspire_App.Web.Services.Courses;

public class EnrollmentApiService: IEnrollmentApiService
{
  private const string CoursesUri = "/api/courses";
  private readonly HttpClient _httpClient;

  public EnrollmentApiService(HttpClient httpClient) => _httpClient = httpClient;

  public async Task EnrollToCourse(Guid courseId, Guid classId, CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/classes/{classId}/enroll",
      new ChangeCourseEnrollmentRequest(), cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public async Task LeaveCourse(Guid courseId, Guid classId, CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/classes/{classId}/unenroll",
      new ChangeCourseEnrollmentRequest { }, cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public Task LeaveCourseByAdmin(string courseId, string classId, string studentId,
    CancellationToken cancellationToken = default) =>
    throw new NotImplementedException();

  public Task EnrollToCourseByAdmin(string courseId, string classId, string studentId,
    CancellationToken cancellationToken = default) =>
    throw new NotImplementedException();
}
