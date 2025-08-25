using Grpc.Core;

using Library.GRPC;

using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;

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
    var studentResult = await _mediator.Send(request.MapToGetStudentQuery());
    return studentResult.Match(
      data => data.MapToGrpcStudentResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListStudentsResponse> ListStudents(GrpcListStudentsRequest request,
    ServerCallContext context)
  {
    var studentsResult = await _mediator.Send(request.MapToListStudentsQuery());
    return studentsResult.Match(
      data => data.MapToGrpcListStudentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedResponse> DeleteStudent(GrpcDeleteStudentRequest request, ServerCallContext context)
  {
    var deleteStudentResult = await _mediator.Send(request.MapToDeleteStudentCommand());
    return deleteStudentResult.Match(
      _ => new GrpcUpdatedResponse { Updated = true, Message = "Student deleted successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
