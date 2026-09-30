using EnrollmentsGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Enrollments.Features.Enrollments.GetClassEnrollments;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.GetStudentEnrollments;

namespace Service.Enrollments.Features.Enrollments;

/// <summary>
/// gRPC entry point for reading enrollments. It only translates gRPC messages; the work is done by the handlers.
/// </summary>
public class EnrollmentsService : GrpcEnrollmentsService.GrpcEnrollmentsServiceBase
{
  private readonly ILogger<EnrollmentsService> _logger;
  private readonly IMediator _mediator;

  public EnrollmentsService(IMediator mediator, ILogger<EnrollmentsService> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public override async Task<GrpcListEnrollmentsResponse> GetClassEnrollments(GrpcGetClassEnrollmentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToGetClassEnrollmentsQuery(), context.CancellationToken);
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcListEnrollmentsResponse> GetCourseEnrollments(GrpcGetCourseEnrollmentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToGetCourseEnrollmentsQuery(), context.CancellationToken);
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcListEnrollmentsResponse> GetStudentEnrollments(
    GrpcGetStudentEnrollmentsRequest request, ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToGetStudentEnrollmentsQuery(), context.CancellationToken);
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }
}
