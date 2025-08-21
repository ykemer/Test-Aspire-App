using System.Security.Claims;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Google.Protobuf.Collections;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;
using Platform.Services.User;

namespace Platform.Features.Courses.ListCourse;

public class ListCoursesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<CourseListItemResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IUserService _userService;

  public ListCoursesEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IUserService userService, GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _userService = userService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
  }

  public override void Configure()
  {
    Get("/api/courses");
    Policies("RequireUserRole");
    Claims("UserId");
    Claims(ClaimTypes.Role);
    ResponseCache(60);
    Options(x => x.RequireRateLimiting("fixed-per-user"));
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

    if (_userService.IsAdmin(User))
    {
      return courses.ToCourseListItemResponse();
    }

    var request = new GrpcGetEnrollmentsByCoursesRequest
    {
      CourseIds = { courses.Items.Select(c => c.Id) }, StudentId = _userService.GetUserId(User).ToString()
    };

    var enrollmentsRequest =
      _enrollmentsGrpcService.GetEnrollmentsByCoursesAsync(request, cancellationToken: ct);

    var enrollmentsResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);
    if (enrollmentsResult.IsError)
    {
      return enrollmentsResult.FirstError;
    }

    var enrollments = enrollmentsResult.Value.Items;

    return courses.ToCourseListItemResponse(enrollments.ToList());
  }
}
