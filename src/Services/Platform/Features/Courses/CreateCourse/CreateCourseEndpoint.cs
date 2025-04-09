using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.CreateCourse;

public class CreateCourseEndpoint : Endpoint<CreateCourseRequest,
  ErrorOr<CourseResponse>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IOutputCacheStore _outputCache;

  public CreateCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IOutputCacheStore outputCache)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _outputCache = outputCache;
  }

  public override void Configure()
  {
    Post("/api/courses/create");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(CreateCourseRequest createCourseCommand,
    CancellationToken ct)
  {
    var request =
      _coursesGrpcService.CreateCourseAsync(createCourseCommand.ToGrpcCreateCourseRequest(), cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    await _outputCache.EvictByTagAsync("courses", ct);
    return result.Match<ErrorOr<CourseResponse>>(
      data => data.ToCourseResponse(),
      error => error);
  }
}
