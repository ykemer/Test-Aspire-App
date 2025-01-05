using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Service.Enrollments.Entities;

[Index(nameof(StudentId))]
[Index(nameof(CourseId))]
public class Enrollment
{
    [Key] public string Id { get; init; } = Guid.NewGuid().ToString();

    public DateTime EnrollmentDateTime { get; set; } = DateTime.Now;

   
    public string CourseId { get; set; }
    public string StudentId { get; set; }
    
    public string StudentFirstName { get; set; }
    public string StudentLastName { get; set; }
}