using Contracts.Students.Responses;

namespace Contracts.Courses.Responses;

public class CourseResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }
   
    // TODO remove and fetch from Enrollments
    public List<StudentResponse>? EnrolledStudents { get; set; }
}