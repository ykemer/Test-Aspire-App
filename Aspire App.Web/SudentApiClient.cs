namespace Aspire_App.Web;

public class StudentApiClient(HttpClient httpClient)
{
    public async Task<StudentListItem[]> GetStudentsListAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<StudentListItem>? students = null;

        await foreach (var student in httpClient.GetFromJsonAsAsyncEnumerable<StudentListItem>("/students", cancellationToken))
        {
            if (students?.Count >= maxItems)
            {
                break;
            }
            if (student is not null)
            {
                students ??= [];
                students.Add(student);
            }
        }

        return students?.ToArray() ?? [];
    }

    public async Task<StudentListItem?> CreateStudent(StudentListItem student,  CancellationToken cancellationToken = default)
    {
        var result = await httpClient.PostAsJsonAsync("/students", student, cancellationToken);
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadFromJsonAsync<StudentListItem>(cancellationToken: cancellationToken)  ;
       
    }
}





public record StudentListItem(
 string FirstName,
 string LastSurname,
 string Email,
 DateTime DateOfBirth
);