using EnrollmentsGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Enrollments.Entities;
using Service.Enrollments.Middleware;

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

  public override async Task<GrpcListEnrollmentsResponse> GetCourseEnrollments(GrpcGetCourseEnrollmentsRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToGetCourseEnrollmentsRequest());
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListEnrollmentsResponse> GetEnrollmentsByCourses(
    GrpcGetEnrollmentsByCoursesRequest request, ServerCallContext context)
  {
    var result =
      await _mediator.Send(request.MapToListOfEnrollmentsByCoursesQuery());
    return result.Match(
      enrollments => enrollments.MapToGrpcListEnrollmentsResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }


  public override async Task<GrpcUpdateEnrollmentResponse> EnrollStudent(GrpcEnrollStudentRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToEnrollStudentToCourseCommand());
    return result.Match(
      _ => new GrpcUpdateEnrollmentResponse { Success = true, Message = "Student enrolled successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdateEnrollmentResponse> DeleteEnrollment(GrpcDeleteEnrollmentRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToUnenrollStudentFromCourseCommand());
    return result.Match(
      _ => new GrpcUpdateEnrollmentResponse { Success = true, Message = "Student enrolled successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
