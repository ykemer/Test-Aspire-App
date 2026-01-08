using ClassesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Classes.GetClass;
using Service.Courses.Features.Classes.ListClasses;
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
}
