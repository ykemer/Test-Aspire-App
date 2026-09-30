using Contracts.Students.Events;

using Grpc.Core;

using Library.GRPC;

using Rebus.Bus;

using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudents;

using StudentsGRPC;

namespace Service.Students.Features;

/// <summary>
/// gRPC entry point for students. It only translates gRPC messages and publishes events;
/// the work is done by the handlers.
/// Students are not created here: Platform publishes a "user created" event on registration.
/// </summary>
public class StudentsService : GrpcStudentsService.GrpcStudentsServiceBase
{
  private readonly IBus _bus;
  private readonly ILogger<StudentsService> _logger;
  private readonly IMediator _mediator;

  public StudentsService(ILogger<StudentsService> logger, IMediator mediator, IBus bus)
  {
    _logger = logger;
    _mediator = mediator;
    _bus = bus;
  }

  public override async Task<GrpcStudentResponse> GetStudentById(GrpcGetStudentByIdRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToGetStudentQuery(), context.CancellationToken);
    return result.Match(
      student => student.MapToGrpcStudentResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcListStudentsResponse> ListStudents(GrpcListStudentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToListStudentsQuery(), context.CancellationToken);
    return result.Match(
      students => students.MapToGrpcListStudentsResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcUpdatedResponse> DeleteStudent(GrpcDeleteStudentRequest request,
    ServerCallContext context)
  {
    var command = request.ToDeleteStudentCommand();
    var result = await _mediator.Send(command, context.CancellationToken);
    if (result.IsError)
    {
      throw GrpcErrorHandler.ThrowAndLogRpcException(result.Errors, _logger);
    }

    await _bus.Publish(new StudentDeletedEvent { StudentId = command.StudentId });
    return new GrpcUpdatedResponse { Updated = true, Message = "Student deleted successfully" };
  }
}
