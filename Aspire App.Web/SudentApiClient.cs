using System.Text.Json;
using Library.Pagination;

namespace Aspire_App.Web;

public class StudentApiClient(HttpClient httpClient)
{
    public async Task<PagedList<StudentListItem>> GetStudentsListAsync(int page, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await httpClient.GetAsync($"/api/students?page={page}&pageSize={pageSize}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Error fetching attributes");
        }
     
        var res = await response.Content.ReadFromJsonAsync<PagedList<StudentListItem>>(options);
        return res;
    }

    public async Task<StudentListItem?> CreateStudent(StudentListItem student, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.PostAsJsonAsync("/api/students", student, cancellationToken);
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadFromJsonAsync<StudentListItem>(cancellationToken: cancellationToken);
    }
}





public class StudentListItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }
}
