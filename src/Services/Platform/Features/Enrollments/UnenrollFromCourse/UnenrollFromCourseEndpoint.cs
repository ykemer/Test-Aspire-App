using Contracts.Courses.Requests;
using Contracts.Enrollments.Events;

using EnrollmentsGRPCClient;

using FastEndpoints;

using MassTransit;

using Platform.Middleware.Grpc;
using Platform.Services.User;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.UnenrollFromCourse;

public class UnenrollFromCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest,
  ErrorOr<Deleted>>
{
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;
  private readonly IPublishEndpoint _publishEndpoint;

  public UnenrollFromCourseEndpoint(IUserService userService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService, IPublishEndpoint publishEndpoint)
  {
    _userService = userService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _publishEndpoint = publishEndpoint;
  }

  public override void Configure()
  {
    Post("/api/courses/unenroll");
    Policies("RequireUserRole");
    Claims("UserId");
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
    CancellationToken ct)
  {
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

    var unerollmentRequest =
      _enrollmentsGrpcService.DeleteEnrollmentAsync(new GrpcDeleteEnrollmentRequest
      {
        CourseId = request.CourseId.ToString(), StudentId = userId.ToString()
      });

    var unenrollmentResponse =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(unerollmentRequest, ct);

    if (unenrollmentResponse.IsError)
    {
      return unenrollmentResponse.Errors[0];
    }

    await _publishEndpoint.Publish(
      new StudentEnrollmentCountChangedEvent
      {
        StudentId = userId.ToString(), CourseId = request.CourseId.ToString(), IsIncrease = false
      }, ct);

    return Result.Deleted;
  }
}
