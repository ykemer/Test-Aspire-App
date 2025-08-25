using System.Text.Json;

using Aspire_App.Web.Helpers;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Classes;
using Contracts.Courses.Requests.Courses;
using Contracts.Courses.Responses;
using Contracts.Enrollments.Responses;

namespace Aspire_App.Web.Services.Courses;

public class ClassesApiApiService : IClassesApiService
{
  private const string CoursesUri = "/api/courses";
  private readonly HttpClient _httpClient;

  public ClassesApiApiService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<PagedList<ClassListItemResponse>> GetClassListAsync(string courseId, int page, int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response =
      await _httpClient.GetAsync($"{CoursesUri}/{courseId}/classes?page={page}&pageSize={pageSize}", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching class lists");
    }

    return await response.Content.ReadFromJsonAsync<PagedList<ClassListItemResponse>>(options);
  }

  public async Task<ClassResponse> GetClass(Guid courseId, Guid classId, CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response = await _httpClient.GetAsync($"/api/courses/{courseId}/classes/{classId}", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching class");
    }

    return await response.Content.ReadFromJsonAsync<ClassResponse>(options);
  }

  public async Task<List<EnrollmentResponse>> GetClassEnrollments(Guid courseId, Guid classId,
    CancellationToken cancellationToken = default)
  {
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var response =
      await _httpClient.GetAsync($"/api/courses/{courseId}/classes/{classId}/enrollments", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ArgumentException("Error fetching enrollments");
    }

    return await response.Content.ReadFromJsonAsync<List<EnrollmentResponse>>(options);
  }

  public async Task CreateClass(Guid courseId, CreateClassRequest createClassRequest,
    CancellationToken cancellationToken = default)
  {
    var command = new CreateClassRequest
    {
      RegistrationDeadline = createClassRequest.RegistrationDeadline.ToUniversalTime(),
      CourseStartDate = createClassRequest.CourseStartDate.ToUniversalTime(),
      CourseEndDate = createClassRequest.CourseEndDate.ToUniversalTime(),
      MaxStudents = createClassRequest.MaxStudents,
    };

    var response =
      await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/classes", command, cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }


  public async Task UpdateClass(Guid courseId, Guid classId, UpdateClassRequest updateCourseRequest,
    CancellationToken cancellationToken = default)
  {

    var command = new UpdateClassRequest
    {
      RegistrationDeadline = updateCourseRequest.RegistrationDeadline.ToUniversalTime(),
      CourseStartDate = updateCourseRequest.CourseStartDate.ToUniversalTime(),
      CourseEndDate = updateCourseRequest.CourseEndDate.ToUniversalTime(),
      MaxStudents = updateCourseRequest.MaxStudents,
    };
    var response =
      await _httpClient.PostAsJsonAsync($"/api/courses/{courseId}/classes/{classId}", command,
        cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }

  public async Task DeleteClass(Guid courseId, Guid classId, CancellationToken cancellationToken = default)
  {
    var response =
      await _httpClient.DeleteAsync($"/api/courses/{courseId.ToString()}/classes/{classId.ToString()}",
        cancellationToken);
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    await FrontendHelper.ProcessValidationDetails(response);
  }
}
