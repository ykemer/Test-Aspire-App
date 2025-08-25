using Aspire_App.Web.Helpers;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Courses;
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
    var response = await _httpClient.GetAsync($"{CoursesUri}?page={page}&pageSize={pageSize}", cancellationToken);
    return await FrontendHelper.ReadJsonOrThrowForErrors<PagedList<CourseListItemResponse>>(response, "Courses not found");
  }

  public async Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.GetAsync($"/api/courses/{guid}", cancellationToken);
    return await FrontendHelper.ReadJsonOrThrowForErrors<CourseResponse>(response, "Course not found");
  }

  public async Task<List<EnrollmentResponse>> GetCourseEnrollments(Guid guid,
    CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.GetAsync($"/api/courses/{guid}/enrollments", cancellationToken);
    return await FrontendHelper.ReadJsonOrThrowForErrors<List<EnrollmentResponse>>(response, "Enrollments not found");
  }


  public async Task CreateCourse(CreateCourseRequest createCourseRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PostAsJsonAsync("/api/courses", createCourseRequest, cancellationToken);
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

  public async Task UpdateCourse(Guid courseId, UpdateCourseRequest updateCourseRequest,
    CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.PatchAsJsonAsync($"/api/courses/{courseId.ToString()}", updateCourseRequest, cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }
}
