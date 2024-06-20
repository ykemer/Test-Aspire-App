using System.Text.Json;
using Aspire_App.Web.Models.Courses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Courses;

public class CoursesApiService : ICoursesApiService
{
    private readonly HttpClient _httpClient;

    public CoursesApiService( HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<PagedList<CoursesListItem>> GetCoursesListAsync(int page, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

         var response = await _httpClient.GetAsync($"/api/courses?page={page}&pageSize={pageSize}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Error fetching attributes");
        }
     
        var res = await response.Content.ReadFromJsonAsync<PagedList<CoursesListItem>>(options);
        return res;
    }

    public async Task<CoursesListItem> GetCourse(Guid guid, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await _httpClient.GetAsync($"/api/course/{guid}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Error fetching attributes");
        }
     
        var res = await response.Content.ReadFromJsonAsync<CoursesListItem>(options);
        return res;
    }
}






