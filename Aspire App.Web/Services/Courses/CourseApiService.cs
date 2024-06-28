using System.Text.Json;
using Aspire_App.Web.Contracts.Requests.Courses;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Courses;

public class CoursesApiService : ICoursesApiService
{
    private const string AdminUri = "/api/admin/courses";
    private const string StudentUri = "/api/courses";
    private readonly HttpClient _httpClient;

    public CoursesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<PagedList<CourseResponse>> GetCoursesListForStudentAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetCoursesListAsync(StudentUri, page, pageSize, cancellationToken);
    }

    public async Task<PagedList<CourseResponse>> GetCoursesListForAdminAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await GetCoursesListAsync(AdminUri, page, pageSize, cancellationToken);
    }


    public async Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await _httpClient.GetAsync($"/api/admin/courses/{guid}", cancellationToken);

        if (!response.IsSuccessStatusCode) throw new ApplicationException("Error fetching attributes");

        return await response.Content.ReadFromJsonAsync<CourseResponse>(options);
    }

    public async Task EnrollToCourse(Guid courseId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/courses/enroll", new ChangeEnrollmentForTheCourse{ CourseId = courseId}, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task LeaveCourse(Guid courseId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/courses/leave", new ChangeEnrollmentForTheCourse{ CourseId = courseId}, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task EnrollToCourseByAdmin(ChangeEnrollmentForTheCourseByAdmin request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/admin/courses/enroll", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task LeaveCourseByAdmin(ChangeEnrollmentForTheCourseByAdmin request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/admin/courses/leave", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/courses/create", createCourseRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }


    private async Task<PagedList<CourseResponse>> GetCoursesListAsync(string url, int page, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await _httpClient.GetAsync($"{url}/list?page={page}&pageSize={pageSize}", cancellationToken);

        if (!response.IsSuccessStatusCode) throw new ApplicationException("Error fetching attributes");

        return await response.Content.ReadFromJsonAsync<PagedList<CourseResponse>>(options);
    }
    
    
}