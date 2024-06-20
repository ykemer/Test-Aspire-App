using System.Text.Json;
using Aspire_App.Web.Models.Students;
using Library.Pagination;

namespace Aspire_App.Web.Services.Students;

public class StudentApiService : IStudentApiService
{
    private readonly HttpClient _httpClient;

    public StudentApiService( HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<PagedList<StudentListItem>> GetStudentsListAsync(int page, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

         var response = await _httpClient.GetAsync($"/api/students?page={page}&pageSize={pageSize}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Error fetching attributes");
        }
     
        var res = await response.Content.ReadFromJsonAsync<PagedList<StudentListItem>>(options);
        return res;
    }

   
}






