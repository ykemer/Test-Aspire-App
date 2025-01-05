using EnrollmentsGRPC;
using Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;
using Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

namespace Service.Enrollments.Middleware;

public static class GrpcRequestsExtensionMethods
{
    public static GetCourseEnrollmentsRequest ToGetCourseEnrollmentsRequest(this GrpcGetCourseEnrollmentsRequest request)
    {
        return new GetCourseEnrollmentsRequest(request.CourseId);
    }
    
    public static ListOfEnrollmentsByCoursesQuery ToListOfEnrollmentsByCoursesQuery(this GrpcGetEnrollmentsByCoursesRequest request)
    {
        return new ListOfEnrollmentsByCoursesQuery(request.CourseIds.ToList(), request.StudentId);
    }
    
    public static EnrollStudentToCourseCommand ToEnrollStudentToCourseCommand(this GrpcEnrollStudentRequest request)
    {
        return new EnrollStudentToCourseCommand(request.CourseId, request.StudentId, request.StudentFirstName, request.StudentLastName);
    }
    
    public static UnenrollStudentFromCourseCommand ToGrpcDeleteEnrollmentRequest(this GrpcDeleteEnrollmentRequest command)
    {
        return new UnenrollStudentFromCourseCommand(command.CourseId, command.StudentId);
    }
}