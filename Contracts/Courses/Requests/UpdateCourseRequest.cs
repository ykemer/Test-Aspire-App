namespace Contracts.Courses.Requests;

public class UpdateCourseRequest: CreateCourseRequest
{
    public string Id { get; set; }
}