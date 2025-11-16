using CoursesGRPC;

using Grpc.Core;

using Library.GRPC;

using Service.Courses.Features.Courses.CreateCourse;
using Service.Courses.Features.Courses.DeleteCourse;
using Service.Courses.Features.Courses.GetCourse;
using Service.Courses.Features.Courses.ListCourses;
using Service.Courses.Features.Courses.UpdateCourse;

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

  public override async Task<GrpcCourseResponse> CreateCourse(GrpcCreateCourseRequest request,
    ServerCallContext context)
  {
    var output =
      await _mediator.Send(request.MapToCreateCourseCommand());

    return output.Match(
      course =>
      {
        _logger.LogTrace("Course {CourseName} is being created", request.Name);
        return course.MapToGrpcCourseResponse();
      },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedCourseResponse> UpdateCourse(GrpcUpdateCourseRequest request,
    ServerCallContext context)
  {
    var result = await _mediator.Send(request.MapToUpdateCourseCommand());

    return result.Match(
      _ => new GrpcUpdatedCourseResponse { Message = "Course updated successfully", Success = true },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }

  public override async Task<GrpcUpdatedCourseResponse> DeleteCourse(GrpcDeleteCourseRequest request,
    ServerCallContext context)
  {
    var output = await _mediator.Send(request.MapToDeleteCourseCommand());
    return output.Match(
      _ => new GrpcUpdatedCourseResponse { Success = true, Message = "Course deleted successfully" },
      error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
  }
}
