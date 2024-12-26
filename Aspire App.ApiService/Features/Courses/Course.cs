using System.ComponentModel.DataAnnotations;
using Aspire_App.ApiService.Features.Enrollments;

namespace Aspire_App.ApiService.Features.Courses;

public class Course
{
    [Key] public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; }
    public string Description { get; set; }

    public IList<StudentEnrollment> StudentEnrollments { get; } = new List<StudentEnrollment>();
}