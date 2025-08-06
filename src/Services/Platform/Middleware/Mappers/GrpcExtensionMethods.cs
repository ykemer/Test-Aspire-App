using Contracts.Common;
using Contracts.Courses.Responses;
using Contracts.Enrollments.Responses;
using Contracts.Students.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using Google.Protobuf.Collections;

using StudentsGRPCClient;

namespace Platform.Middleware.Mappers;

public static class GrpcExtensionMethods
{
  public static StudentResponse ToStudentResponse(this GrpcStudentResponse student) =>
    new()
    {
      Id = Guid.Parse(student.Id),
      FirstName = student.FirstName,
      LastName = student.LastName,
      Email = student.Email,
      DateOfBirth = student.Birthday.ToDateTime(),
      EnrollmentCount = student.EnrolledCourses
    };

  public static PagedList<StudentResponse> ToStudentListResponse(this GrpcListStudentsResponse response) =>
    new()
    {
      Items = response.Items.Select(s => s.ToStudentResponse()).ToList(),
      CurrentPage = response.CurrentPage,
      TotalPages = response.TotalPages,
      PageSize = response.PageSize,
      TotalCount = response.TotalCount
    };

  public static List<EnrollmentResponse> ToEnrollmentResponseList(this GrpcListEnrollmentsResponse response) =>
    response.Items.Select(i => new EnrollmentResponse
    {
      Id = i.Id,
      CourseId = i.CourseId,
      StudentId = i.StudentId,
      EnrollmentDateTime = i.EnrollmentDateTime.ToDateTime(),
      FirstName = i.StudentFirstName,
      LastName = i.StudentLastName
    }).ToList();

  public static CourseResponse ToCourseResponse(this GrpcCourseResponse course) =>
    new()
    {
      Id = Guid.Parse(course.Id),
      Description = course.Description,
      Name = course.Name,
      EnrollmentsCount = course.TotalStudents
    };

  public static PagedList<CourseListItemResponse> ToCourseListItemResponse(this GrpcListCoursesResponse course, List<GrpcEnrollmentResponse>? enrollments = null) =>
    new()
    {
      Items = course.Items.Select(i => new CourseListItemResponse
      {
        Id = Guid.Parse(i.Id),
        Name = i.Name,
        Description = i.Description,
        EnrollmentsCount = i.TotalStudents,
        IsUserEnrolled = enrollments?.Any(e => e.CourseId == i.Id) == true
      }).ToList(),
      CurrentPage = course.CurrentPage,
      TotalPages = course.TotalPages,
      PageSize = course.PageSize,
      TotalCount = course.TotalCount,

    };
}
