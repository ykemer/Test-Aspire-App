using System.ComponentModel.DataAnnotations;

namespace Service.Courses.Entities;

public class Course
{
    [Key] [Required] public string Id { get; init; } = Guid.NewGuid().ToString();

    [Required] public string Name { get; init; }

    [Required] public string Description { get; init; }

    public int EnrollmentsCount { get; set; } = 0;
}