using System;
using System.Text.Json;
using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

namespace Aspire_App.Web.Services.Courses;

public class CoursesApiService : ICoursesApiService
{
    private const string CoursesUri = "/api/courses";
    private readonly HttpClient _httpClient;

    public CoursesApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<PagedList<CourseListItemResponse>> GetCoursesListAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response =
            await _httpClient.GetAsync($"{CoursesUri}/list?page={page}&pageSize={pageSize}", cancellationToken);

        if (!response.IsSuccessStatusCode) throw new ApplicationException("Error fetching attributes");

        return await response.Content.ReadFromJsonAsync<PagedList<CourseListItemResponse>>(options);
    }


    public async Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await _httpClient.GetAsync($"/api/courses/{guid}", cancellationToken);

        if (!response.IsSuccessStatusCode) throw new ApplicationException("Error fetching attributes");

        return await response.Content.ReadFromJsonAsync<CourseResponse>(options);
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
        var response = await _httpClient.PostAsJsonAsync("/api/courses/enroll", adminRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task LeaveCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/courses/leave", adminRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateCourse(CreateCourseRequest createCourseRequest,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/courses/create", createCourseRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}