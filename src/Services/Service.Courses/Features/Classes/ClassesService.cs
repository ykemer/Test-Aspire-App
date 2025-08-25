using ClassesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Classes.CreateClass;
using Service.Courses.Features.Classes.DeleteClass;
using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;
using Service.Courses.Features.Classes.UpdateClass;
using Service.Courses.Features.Courses;

using static ClassesGRPC.GrpcClassService;

namespace Service.Courses.Features.Classes;

public class ClassesService : GrpcClassServiceBase
{
  private readonly ILogger<ClassesService> _logger;
  private readonly IMediator _mediator;

  public ClassesService(ILogger<ClassesService> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  public override async Task<GrpcClassResponse> GetClass(GrpcGetClassRequest request, ServerCallContext context)
  {
    var output = await _mediator.Send(request.ToGetClassQuery());
    return output.Match(
      course => course.MapToGrpcClassResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListClassResponse> ListClasses(GrpcListClassRequest request, ServerCallContext context)
  {
    var output = await _mediator.Send(request.MapToListClassesRequest());
    return output.Match(value => value.MapToGrpcListClassResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcClassResponse> CreateClass(GrpcCreateClassRequest request, ServerCallContext context)
  {
    var output =
      await _mediator.Send(request.MapToCreateClassCommand());

    return output.Match(
      course =>
      {
        _logger.LogTrace("Class for course {CourseId} with start date {StartDate} is being created", request.CourseId, request.CourseStartDate);
        return course.MapToGrpcClassResponse();
      },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedClassResponse> UpdateClass(GrpcUpdateClassRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToUpdateClassCommand());

    return result.Match(
      _ => new GrpcUpdatedClassResponse { Message = "Class updated successfully", Success = true },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedClassResponse> DeleteClass(GrpcDeleteClassRequest request,
    ServerCallContext context)
  {
    var output = await _mediator.Send(request.MapToDeleteClassCommand());
    return output.Match(
      _ => new GrpcUpdatedClassResponse { Success = true, Message = "Class deleted successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
