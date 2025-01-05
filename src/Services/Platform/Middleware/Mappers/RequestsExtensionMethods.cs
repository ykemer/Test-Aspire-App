using Contracts.Courses.Requests;
using Contracts.Students.Requests;
using Contracts.Users.Events;
using Contracts.Users.Requests;
using CoursesGRPCClient;
using EnrollmentsGRPCClient;
using Platform.Entities;
using StudentsGRPCClient;

namespace Platform.Middleware.Mappers;

public static class RequestsExtensionMethods
{
    public static GrpcListStudentsRequest ToGrpcListStudentsRequest(this ListStudentsRequest request)
    {
        return new GrpcListStudentsRequest
        {
            Page = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    
    public static GrpcCreateCourseRequest ToGrpcCreateCourseRequest(this CreateCourseRequest createCourseCommand)
    {
        return new GrpcCreateCourseRequest
        {
            Name = createCourseCommand.Name,
            Description = createCourseCommand.Description
        };
    }
    
    public static GrpcDeleteCourseRequest ToGrpcDeleteCourseRequest(this DeleteCourseRequest deleteCourseCommand)
    {
        return new GrpcDeleteCourseRequest
        {
            Id = deleteCourseCommand.Id.ToString()
        };
    }
    
    public static GrpcListCoursesRequest ToGrpcGetEnrollmentsByCoursesRequest(this ListCoursesRequest request)
    {
        return new GrpcListCoursesRequest
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Query = request.Query
        };
    }
   
    public static GrpcUpdateCourseRequest ToGrpcUpdateCourseRequest(this UpdateCourseRequest updateCourseCommand)
    {
        return new GrpcUpdateCourseRequest
        {
            Id = updateCourseCommand.Id,
            Name = updateCourseCommand.Name,
            Description = updateCourseCommand.Description
        };
    }
    
    public static ApplicationUser ToApplicationUser(this UserRegisterRequest request)
    {
        return new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth
        };
    }
    
    public static UserCreatedEvent ToUserCreatedEvent(this ApplicationUser user)
    {
        return new UserCreatedEvent
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email
        };
    }
}