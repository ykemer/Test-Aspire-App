using System.Security.Claims;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

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

  public ListCoursesEndpoint(IUserService userService, GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _userService = userService;
    _coursesGrpcService = coursesGrpcService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Get("/api/courses/list");
    Policies("RequireUserRole");
    Claims("UserId");
    Claims(ClaimTypes.Role);
    ResponseCache(60);
  }

  public override async Task<ErrorOr<PagedList<CourseListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var userId = _userService.GetUserId(User);

    var coursesRequest =
      _coursesGrpcService.ListCoursesAsync(query.ToGrpcGetEnrollmentsByCoursesRequest(), cancellationToken: ct);

    var coursesResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);

    if (coursesResult.IsError)
    {
      return coursesResult.FirstError;
    }

    var courses = coursesResult.Value;
    var fetchedCourseIds = courses.Items.Select(i => i.Id).ToList();
    var enrollmentsRequest =
      _enrollmentsGrpcService.GetEnrollmentsByCoursesAsync(
        new GrpcGetEnrollmentsByCoursesRequest { CourseIds = { fetchedCourseIds }, StudentId = userId.ToString() },
        cancellationToken: ct);

    var enrollmentsResult = await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);
    if (enrollmentsResult.IsError)
    {
      return enrollmentsResult.FirstError;
    }


    var enrollments = enrollmentsResult.Value;
    var mappedList = courses.ToCourseListItemResponse(enrollments);
    return PagedList<CourseListItemResponse>.Create(mappedList, query.PageNumber, query.PageSize);
  }
}
