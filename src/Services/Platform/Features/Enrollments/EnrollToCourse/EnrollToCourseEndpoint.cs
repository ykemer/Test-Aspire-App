using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Requests;

using FastEndpoints;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

using Rebus.Bus;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;

  public EnrollToCourseEndpoint(
    IUserService userService,
    IGrpcRequestMiddleware grpcRequestMiddleware,
    GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    IBus bus)
  {
    _userService = userService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _bus = bus;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes/{ClassId}/enroll");
    Policies("RequireUserRole");
    Description(x => x.WithTags("Enrollments"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request, CancellationToken ct)
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
    var studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    if (studentResponse.IsError)
    {
      return studentResponse.Errors[0];
    }

    var student = studentResponse.Value;

    await _bus.Send(new CreateEnrollmentCommand
    {
      CourseId = courseId,
      ClassId = classId,
      StudentId = userId,
      FirstName = student.FirstName,
      LastName = student.LastName
    });

    return Result.Updated;
  }
}
