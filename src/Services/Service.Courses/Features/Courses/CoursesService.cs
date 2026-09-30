using CoursesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;

namespace Service.Courses.Features.Courses;

/// <summary>
/// gRPC entry point for reading courses. It only translates gRPC messages; the work is done by the handlers.
/// </summary>
public class CoursesService : GrpcCoursesService.GrpcCoursesServiceBase
{
  private readonly ILogger<CoursesService> _logger;
  private readonly IMediator _mediator;

  public CoursesService(ILogger<CoursesService> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  public override async Task<GrpcCourseResponse> GetCourse(GrpcGetCourseRequest request, ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToGetCourseQuery(), context.CancellationToken);
    return result.Match(
      course => course.MapToGrpcCourseResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }

  public override async Task<GrpcListCoursesResponse> ListCourses(GrpcListCoursesRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.ToListCoursesQuery(), context.CancellationToken);
    return result.Match(
      courses => courses.MapToGrpcListCoursesResponse(),
      errors => throw GrpcErrorHandler.ThrowAndLogRpcException(errors, _logger));
  }
}
