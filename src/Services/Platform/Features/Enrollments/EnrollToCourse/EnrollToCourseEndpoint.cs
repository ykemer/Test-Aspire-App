using ClassesGRPCClient;

using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Enrollments;
using Contracts.Enrollments.Events;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using MassTransit;

using Platform.Middleware.Grpc;
using Platform.Services.User;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;
  private readonly IPublishEndpoint _publishEndpoint;


  public EnrollToCourseEndpoint(
    IUserService userService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware,
    GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    IPublishEndpoint publishEndpoint, GrpcClassService.GrpcClassServiceClient classGrpcService)
  {
    _userService = userService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _publishEndpoint = publishEndpoint;
    _classGrpcService = classGrpcService;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes/{ClassId}/enroll");
    Policies("RequireUserRole");
    Claims("UserId");
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");

    var userId = _userService.IsAdmin(User) ? request.StudentId : _userService.GetUserId(User);
    if (userId == Guid.Empty)
    {
      return Error.Failure(description: "User not found");
    }


    var studentRequest =
      _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest { Id = userId.ToString() });
    var
      studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    if (studentResponse.IsError)
    {
      return studentResponse.Errors[0];
    }

    var student = studentResponse.Value;

    var enrollmentRequest =
      _enrollmentsGrpcService.EnrollStudentAsync(new GrpcEnrollStudentRequest
      {
        CourseId = courseId.ToString(),
        ClassId = classId.ToString(),
        StudentId = userId.ToString(),
        StudentFirstName = student.FirstName,
        StudentLastName = student.LastName
      });


    var enrollmentResponse =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentRequest, ct);

    if (enrollmentResponse.IsError)
    {
      return enrollmentResponse.Errors[0];
    }

    await _publishEndpoint.Publish(
      new StudentEnrollmentCountChangedEvent
      {
        StudentId = userId.ToString(),
        CourseId = courseId.ToString(),
        ClassId = classId.ToString(),
        IsIncrease = true
      }, ct);


    return Result.Updated;
  }
}
