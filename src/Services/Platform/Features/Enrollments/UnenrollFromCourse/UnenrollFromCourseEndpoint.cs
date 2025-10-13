using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Enrollments;
using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using EnrollmentsGRPCClient;

using FastEndpoints;

using MassTransit;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.UnenrollFromCourse;

public class UnenrollFromCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest,
  ErrorOr<Deleted>>
{
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;

  private readonly ISendEndpointProvider _sendEndpointProvider;

  public UnenrollFromCourseEndpoint(IUserService userService,
    IGrpcRequestMiddleware grpcRequestMiddleware, GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    ISendEndpointProvider sendEndpointProvider)
  {
    _userService = userService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _sendEndpointProvider = sendEndpointProvider;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes/{ClassId}/unenroll");
    Policies("RequireUserRole");
    Claims("UserId");
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
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


    var sendUri = new Uri("queue:delete-enrollment-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);

    await endpoint.Send(new DeleteEnrollmentCommand()
    {
      CourseId = courseId.ToString(), ClassId = classId.ToString(), StudentId = userId.ToString(),
    });

    return Result.Deleted;
  }
}
