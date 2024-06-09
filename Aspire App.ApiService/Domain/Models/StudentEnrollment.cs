using System.ComponentModel.DataAnnotations;

namespace Aspire_App.ApiService.Domain.Models
{
    public class StudentEnrollment
    {
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime Enroled { get; set; } = DateTime.Now;

        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
