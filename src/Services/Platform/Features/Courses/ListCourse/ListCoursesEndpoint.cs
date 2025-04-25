using System.Security.Claims;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.ListCourse;

public class ListCoursesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<CourseListItemResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public ListCoursesEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/courses");
    Policies("RequireUserRole");
    Claims("UserId");
    Claims(ClaimTypes.Role);
    ResponseCache(60);
  }

  [OutputCache(PolicyName = "CoursesCache")]

  public override async Task<ErrorOr<PagedList<CourseListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var coursesRequest =
      _coursesGrpcService.ListCoursesAsync(query.ToGrpcGetEnrollmentsByCoursesRequest(), cancellationToken: ct);

    var coursesResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);

    if (coursesResult.IsError)
    {
      return coursesResult.FirstError;
    }

    var courses = coursesResult.Value;


    return courses.ToCourseListItemResponse();
  }
}
