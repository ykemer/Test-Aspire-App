using System.Text.Json;

using Contracts.Common;
using Contracts.Students.Responses;

namespace Aspire_App.Web.Services.Students;

public class StudentApiService : IStudentApiService
{
  private readonly HttpClient _httpClient;

  public StudentApiService(HttpClient httpClient) => _httpClient = httpClient;


  public async Task<PagedList<StudentResponse>> GetStudentsListAsync(int page, int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    JsonSerializerOptions? options = new() { PropertyNameCaseInsensitive = true };

    var response =
      await _httpClient.GetAsync($"/api/students/list?page={page}&pageSize={pageSize}", cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
      throw new ApplicationException("Error fetching attributes");
    }

    var res = await response.Content.ReadFromJsonAsync<PagedList<StudentResponse>>(options);
    return res;
  }
}
