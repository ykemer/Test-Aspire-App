using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Requests;

using FastEndpoints;

using Platform.Common.Grpc;

using Rebus.Bus;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.UnenrollFromCourse;

/// <summary>
/// Asks the Enrollments service to remove a student from a class. The result arrives later as a live
/// notification, after the unenroll saga has also updated the student and class counters.
/// </summary>
public class UnenrollFromCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Deleted>>
{
  private readonly IBus _bus;
  private readonly IGrpcCaller _grpc;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsClient;

  public UnenrollFromCourseEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsClient,
    IGrpcCaller grpc, IBus bus)
  {
    _studentsClient = studentsClient;
    _grpc = grpc;
    _bus = bus;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId:guid}/classes/{ClassId:guid}/unenroll");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Enrollments"));
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
    CancellationToken ct)
  {
    var studentId = EnrollmentStudent.Resolve(User, request);
    if (studentId.IsError)
    {
      return studentId.Errors;
    }

    // Fail fast with "not found" instead of sending a command that can only be rejected.
    var student = await _grpc.CallAsync(_studentsClient.GetStudentByIdAsync(
      new GrpcGetStudentByIdRequest { Id = studentId.Value.ToString() }, cancellationToken: ct));
    if (student.IsError)
    {
      return student.Errors;
    }

    await _bus.Send(new DeleteEnrollmentCommand
    {
      CourseId = Route<Guid>("CourseId"),
      ClassId = Route<Guid>("ClassId"),
      StudentId = studentId.Value,
      IdempotencyKey = request.IdempotencyKey
    });

    return Result.Deleted;
  }
}
