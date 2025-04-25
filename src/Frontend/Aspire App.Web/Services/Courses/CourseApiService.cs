using System.Text.Json;

using Aspire_App.Web.Helpers;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using Contracts.Enrollments.Responses;

namespace Aspire_App.Web.Services.Courses;

public class CoursesApiService : ICoursesApiService
{
  private const string CoursesUri = "/api/courses";
  private readonly HttpClient _httpClient;

  public CoursesApiService(HttpClient httpClient) => _httpClient = httpClient;


  public async Task<PagedList<CourseListItemResponse>> GetCoursesListAsync(int page, int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response =
      await _httpClient.GetAsync($"{CoursesUri}?page={page}&pageSize={pageSize}", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching course lists");
    }

    return await response.Content.ReadFromJsonAsync<PagedList<CourseListItemResponse>>(options);
  }


  public async Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response = await _httpClient.GetAsync($"/api/courses/{guid}", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching course");
    }

    return await response.Content.ReadFromJsonAsync<CourseResponse>(options);
  }

  public async Task<List<EnrollmentResponse>> GetCourseEnrollments(Guid guid,
    CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response = await _httpClient.GetAsync($"/api/courses/{guid}/enrollments", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching enrollments");
    }

    var output = await response.Content.ReadFromJsonAsync<List<EnrollmentResponse>>(options);
    return output;
  }


  public async Task EnrollToCourse(Guid courseId, CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync("/api/courses/enroll",
      new ChangeCourseEnrollmentRequest { CourseId = courseId }, cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public async Task LeaveCourse(Guid courseId, CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsJsonAsync("/api/courses/leave",
      new ChangeCourseEnrollmentRequest { CourseId = courseId }, cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public async Task EnrollToCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PostAsJsonAsync("/api/courses/enroll", adminRequest, cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public async Task LeaveCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PostAsJsonAsync("/api/courses/unenroll", adminRequest, cancellationToken);
    response.EnsureSuccessStatusCode();
  }

  public async Task CreateCourse(CreateCourseRequest createCourseRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PostAsJsonAsync("/api/courses/create", createCourseRequest, cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }

  public async Task DeleteCourse(Guid id, CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.DeleteAsync($"/api/courses/{id.ToString()}", cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }

  public async Task UpdateCourse(UpdateCourseRequest updateCourseRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PostAsJsonAsync("/api/courses/update", updateCourseRequest, cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }
}
