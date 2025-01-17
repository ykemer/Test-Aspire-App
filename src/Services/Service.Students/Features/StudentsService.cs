using Grpc.Core;

using Library.GRPC;

using Service.Students.Entities;
using Service.Students.Middleware;

using StudentsGRPC;

namespace Service.Students.Features;

public class StudentsService : GrpcStudentsService.GrpcStudentsServiceBase
{
  private readonly ILogger<StudentsService> _logger;
  private readonly IMediator _mediator;

  public StudentsService(ILogger<StudentsService> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  public override async Task<GrpcStudentResponse> GetStudentById(GrpcGetStudentByIdRequest request,
    ServerCallContext context)
  {
    var studentResult = await _mediator.Send(request.ToGetStudentQuery());
    return studentResult.Match(
      data => data.ToGrpcStudentResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override Task<GrpcListStudentsResponse> ListStudents(GrpcListStudentsRequest request,
    ServerCallContext context)
  {
    var studentsResult = _mediator.Send(request.ToListStudentsQuery());

    return studentsResult.Match(
      data => data.ToGrpcListStudentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override Task<GrpcUpdatedResponse> DeleteStudent(GrpcDeleteStudentRequest request, ServerCallContext context)
  {
    var deleteStudentResult = _mediator.Send(request.ToDeleteStudentCommand());
    return deleteStudentResult.Match(
      _ => new GrpcUpdatedResponse { Updated = true, Message = "Student deleted successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
