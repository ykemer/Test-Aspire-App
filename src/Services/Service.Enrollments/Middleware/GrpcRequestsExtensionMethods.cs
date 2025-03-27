using EnrollmentsGRPC;

using Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;
using Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

namespace Service.Enrollments.Middleware;

public static class GrpcRequestsExtensionMethods
{
  public static GetCourseEnrollmentsRequest
    MapToGetCourseEnrollmentsRequest(this GrpcGetCourseEnrollmentsRequest request) => new(request.CourseId);

  public static ListEnrollmentsByCoursesQuery MapToListOfEnrollmentsByCoursesQuery(
    this GrpcGetEnrollmentsByCoursesRequest request) => new(request.CourseIds.ToList(), request.StudentId);

  public static EnrollStudentToCourseCommand MapToEnrollStudentToCourseCommand(this GrpcEnrollStudentRequest request) =>
    new(request.CourseId, request.StudentId, request.StudentFirstName, request.StudentLastName);

  public static UnenrollStudentFromCourseCommand
    MapToUnenrollStudentFromCourseCommand(this GrpcDeleteEnrollmentRequest command) => new(command.CourseId, command.StudentId);
}
