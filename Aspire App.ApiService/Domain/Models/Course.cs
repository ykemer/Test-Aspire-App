using System.ComponentModel.DataAnnotations;

namespace Aspire_App.ApiService.Domain.Models;

public class Course
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; }
    public string Description { get; set; }

    public IList<StudentEnrollment> StudentEnrollments { get; } = new List<StudentEnrollment>();
}