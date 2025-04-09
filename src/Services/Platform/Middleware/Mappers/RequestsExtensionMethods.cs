using Contracts.Courses.Requests;
using Contracts.Students.Requests;
using Contracts.Users.Events;
using Contracts.Users.Requests;

using CoursesGRPCClient;

using Platform.Entities;

using StudentsGRPCClient;

namespace Platform.Middleware.Mappers;

public static class RequestsExtensionMethods
{
  public static GrpcListStudentsRequest ToGrpcListStudentsRequest(this ListStudentsRequest request) =>
    new() { Page = request.PageNumber, PageSize = request.PageSize };

  public static GrpcCreateCourseRequest ToGrpcCreateCourseRequest(this CreateCourseRequest createCourseCommand) =>
    new() { Name = createCourseCommand.Name, Description = createCourseCommand.Description };

  public static GrpcListCoursesRequest ToGrpcGetEnrollmentsByCoursesRequest(this ListCoursesRequest request) =>
    new() { Page = request.PageNumber, PageSize = request.PageSize, Query = request.Query ?? "" };

  public static GrpcUpdateCourseRequest ToGrpcUpdateCourseRequest(this UpdateCourseRequest updateCourseCommand) =>
    new()
    {
      Id = updateCourseCommand.Id, Name = updateCourseCommand.Name, Description = updateCourseCommand.Description
    };

  public static ApplicationUser ToApplicationUser(this UserRegisterRequest request) =>
    new()
    {
      UserName = request.Email,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName,
      DateOfBirth = request.DateOfBirth
    };

  public static UserCreatedEvent ToUserCreatedEvent(this ApplicationUser user) =>
    new()
    {
      Id = user.Id,
      FirstName = user.FirstName,
      LastName = user.LastName,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email
    };
}
