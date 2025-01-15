using Contracts.Common;

using CoursesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Entities;
using Service.Courses.Middleware;

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

  public override async Task<GrpcCourseResponse> CreateCourse(GrpcCreateCourseRequest request,
    ServerCallContext context)
  {
    ErrorOr<Course> output =
      await _mediator.Send(request.ToCreateCourseCommand());

    return output.Match(
      course =>
      {
        _logger.LogTrace("Course {CourseName} is being created", request.Name);
        return course.ToGrpcCourseResponse();
      },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedCourseResponse> UpdateCourse(GrpcUpdateCourseRequest request,
    ServerCallContext context)
  {
    ErrorOr<Updated> result = await _mediator.Send(request.ToUpdateCourseCommand());

    return result.Match(
      _ => new GrpcUpdatedCourseResponse { Message = "Course updated successfully", Success = true },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedCourseResponse> DeleteCourse(GrpcDeleteCourseRequest request,
    ServerCallContext context)
  {
    ErrorOr<Deleted> output = await _mediator.Send(request.ToDeleteCourseCommand());
    return output.Match(
      _ => new GrpcUpdatedCourseResponse { Success = true, Message = "Course deleted successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcCourseResponse> GetCourse(GrpcGetCourseRequest request, ServerCallContext context)
  {
    ErrorOr<Course> output = await _mediator.Send(request.ToGetCourseQuery());
    return output.Match(
      course => course.ToGrpcCourseResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcListCoursesResponse> ListCourses(GrpcListCoursesRequest request,
    ServerCallContext context)
  {
    ErrorOr<PagedList<Course>> output = await _mediator.Send(request.ToListCoursesRequest());
    return output.Match(value => value.ToGrpcListCoursesResponse(),
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
