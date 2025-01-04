using System.ComponentModel.DataAnnotations;

namespace Service.Courses.Entities;

public class Course
{
    [Key] [Required] public string Id { get; init; } = Guid.NewGuid().ToString();

    [Required] public string Name { get; set; }

    [Required] public string Description { get; set; }

    public int EnrollmentsCount { get; set; } = 0;
}