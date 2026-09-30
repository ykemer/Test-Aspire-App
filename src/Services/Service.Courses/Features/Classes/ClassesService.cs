using ClassesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;

namespace Service.Courses.Features.Classes;

/// <summary>
/// gRPC entry point for reading classes. It only translates gRPC messages; the work is done by the handlers.
/// </summary>
public class ClassesService : GrpcClassService.GrpcClassServiceBase
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
    var result = await _mediator.Send(request.ToGetClassQuery(), context.CancellationToken);
    return result.Match(
      courseClass => courseClass.MapToGrpcClassResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcListClassResponse> ListClasses(GrpcListClassRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToListClassesQuery(), context.CancellationToken);
    return result.Match(
      classes => classes.MapToGrpcListClassResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }
}
