using Contracts.Courses.Requests.Enrollments;
using Contracts.Enrollments.Commands;

using FastEndpoints;

using MassTransit;

using Platform.Common.Database;
using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
  private readonly ApplicationDbContext _db;

  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  private readonly ISendEndpointProvider _sendEndpointProvider;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;


  public EnrollToCourseEndpoint(
    IUserService userService,
    IGrpcRequestMiddleware grpcRequestMiddleware,
    GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    ISendEndpointProvider sendEndpointProvider, ApplicationDbContext db)
  {
    _userService = userService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _sendEndpointProvider = sendEndpointProvider;
    _db = db;
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

    var sendUri = new Uri("queue:create-enrollment-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);

    await endpoint.Send(new CreateEnrollmentCommand
    {
      CourseId = courseId.ToString(),
      ClassId = classId.ToString(),
      StudentId = userId.ToString(),
      FirstName = student.FirstName,
      LastName = student.LastName
    });

    return Result.Updated;
  }
}
