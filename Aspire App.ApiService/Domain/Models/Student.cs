using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Aspire_App.ApiService.Domain.Models;

public class Student
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }

    [JsonIgnore] public IList<StudentEnrollment> StudentEnrollements { get; } = new List<StudentEnrollment>();
}