using EnrollmentsGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Enrollments.Common.Database.Entities;
using Service.Enrollments.Features.Enrollments.EnrollStudentToClass;
using Service.Enrollments.Features.Enrollments.GetClassEnrollments;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.GetStudentEnrollments;
using Service.Enrollments.Features.Enrollments.UnenrollStudentFromClass;

namespace Service.Enrollments.Features.Enrollments;

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
    var result = await _mediator.Send(request.MapToGetClassEnrollmentsQuery());
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListEnrollmentsResponse> GetCourseEnrollments(GrpcGetCourseEnrollmentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToGetCourseEnrollmentsQuery());
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListEnrollmentsResponse> GetStudentEnrollments(GrpcGetStudentEnrollmentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToGetStudentEnrollmentsQuery());
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdateEnrollmentResponse> EnrollStudent(GrpcEnrollStudentRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToEnrollStudentToClassCommand());
    return result.Match(
      _ => new GrpcUpdateEnrollmentResponse { Success = true, Message = "Student enrolled successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdateEnrollmentResponse> DeleteEnrollment(GrpcDeleteEnrollmentRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToUnenrollStudentFromClassCommand());
    return result.Match(
      _ => new GrpcUpdateEnrollmentResponse { Success = true, Message = "Student enrolled successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
