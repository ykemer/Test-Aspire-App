using CoursesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;

namespace Service.Courses.Features.Courses;

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
    var output = await _mediator.Send(request.ToGetCourseQuery());
    return output.Match(
      course => course.MapToGrpcCourseResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListCoursesResponse> ListCourses(GrpcListCoursesRequest request,
    ServerCallContext context)
  {
    var command = request.MapToListCoursesRequest();
    var output = await _mediator.Send(command);
    return output.Match(value => value.MapToGrpcListCoursesResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
