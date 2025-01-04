using CoursesGRPC;
using Grpc.Core;
using Library.GRPC;
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

    public override async Task<GrpcCourseResponse> CreateCourse(GrpcCreateCourseRequest request,
        ServerCallContext context)
    {
        var output =
            await _mediator.Send(GrpcDataToCourseMapper.MapGrpcCreateCourseRequestToCreateCourseCommand(request));
        return output.Match(
            course =>
            {
                _logger.LogTrace("Course {CourseName} is being created", request.Name);
                return GrpcDataToCourseMapper.MapCourseToGrpcCourseResponse(course);
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<GrpcUpdatedCourseResponse> UpdateCourse(GrpcUpdateCourseRequest request, ServerCallContext context)
    {
        var updateCourseCommand = new UpdateCourseCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };
        var result = await _mediator.Send(updateCourseCommand);
        return result.Match(
            _ => new GrpcUpdatedCourseResponse
            {
                Message = "Course updated successfully",
                Success = true
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<GrpcUpdatedCourseResponse> DeleteCourse(GrpcDeleteCourseRequest request,
        ServerCallContext context)
    {
        var output = await _mediator.Send(new DeleteCourseCommand(request.Id));
        return output.Match(
            _ => new GrpcUpdatedCourseResponse
            {
                Success = true,
                Message = "Course deleted successfully"
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<GrpcCourseResponse> GetCourse(GrpcGetCourseRequest request, ServerCallContext context)
    {
        var output = await _mediator.Send(new GetCourseQuery(request.Id));
        return output.Match(
            course => GrpcDataToCourseMapper.MapCourseToGrpcCourseResponse(course),
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<GrpcListCoursesResponse> ListCourses(GrpcListCoursesRequest request,
        ServerCallContext context)
    {
        try
        {
            var output = await _mediator.Send(new ListCoursesRequest
            {
                PageSize = request.PageSize,
                PageNumber = request.Page
            });

            return output.Match(value => new GrpcListCoursesResponse
            {
                CurrentPage = value.CurrentPage,
                PageSize = value.PageSize,
                TotalPages = value.TotalPages,
                TotalCount = value.TotalCount,
                Items = { value.Items.Select(GrpcDataToCourseMapper.MapCourseToGrpcCourseResponse) }
            }, error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
        } catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while listing courses");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while listing courses"));
        }
    }
}