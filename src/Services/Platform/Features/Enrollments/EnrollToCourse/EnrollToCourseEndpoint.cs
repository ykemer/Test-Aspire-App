using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Requests;

using FastEndpoints;

using Platform.Common.Grpc;

using Rebus.Bus;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

/// <summary>
/// Asks the Enrollments service to enroll a student in a class. The result arrives later as a live
/// notification, after the enroll saga has also updated the student and class counters.
/// </summary>
public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
  private readonly IBus _bus;
  private readonly IGrpcCaller _grpc;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsClient;

  public EnrollToCourseEndpoint(GrpcStudentsService.GrpcStudentsServiceClient studentsClient, IGrpcCaller grpc,
    IBus bus)
  {
    _studentsClient = studentsClient;
    _grpc = grpc;
    _bus = bus;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId:guid}/classes/{ClassId:guid}/enroll");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Enrollments"));
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
    CancellationToken ct)
  {
    var studentId = EnrollmentStudent.Resolve(User, request);
    if (studentId.IsError)
    {
      return studentId.Errors;
    }

    // The enrollment stores the student's name, so read it from the Students service (this also checks
    // that the student exists).
    var student = await _grpc.CallAsync(_studentsClient.GetStudentByIdAsync(
      new GrpcGetStudentByIdRequest { Id = studentId.Value.ToString() }, cancellationToken: ct));
    if (student.IsError)
    {
      return student.Errors;
    }

    await _bus.Send(new CreateEnrollmentCommand
    {
      CourseId = Route<Guid>("CourseId"),
      ClassId = Route<Guid>("ClassId"),
      StudentId = studentId.Value,
      FirstName = student.Value.FirstName,
      LastName = student.Value.LastName,
      IdempotencyKey = request.IdempotencyKey
    });

    return Result.Updated;
  }
}
