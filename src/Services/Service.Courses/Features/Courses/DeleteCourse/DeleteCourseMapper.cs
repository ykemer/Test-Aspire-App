using CoursesGRPC;

namespace Service.Courses.Features.Courses.DeleteCourse;

public static class DeleteCourseMapper
{
  public static DeleteCourseCommand MapToDeleteCourseCommand(this GrpcDeleteCourseRequest request) => new(request.Id);
}
