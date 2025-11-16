using System.Security.Claims;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

namespace Platform.Features.Courses.ListCourses;

public class ListCoursesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<CourseListItemResponse>>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IUserService _userService;

  public ListCoursesEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IUserService userService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService)
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

    Description(x => x.WithTags("Courses"));
  }

  [OutputCache(PolicyName = "CoursesCache")]
  public override async Task<ErrorOr<PagedList<CourseListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var enrolledClassesList = new List<string>();
    var enrolledCoursesList = new List<string>();


    var isAdmin = User.IsInRole("Administrator");

    if (!isAdmin)
    {
      var request = new GrpcGetStudentEnrollmentsRequest { StudentId = _userService.GetUserId(User).ToString() };

      var enrollmentsRequest =
        _enrollmentsGrpcService.GetStudentEnrollmentsAsync(request, cancellationToken: ct);

      var enrollmentsResult =
        await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);
      if (enrollmentsResult.IsError)
      {
        return enrollmentsResult.FirstError;
      }

      var enrollments = enrollmentsResult.Value.Items;
      enrolledClassesList = enrollments.Select(x => x.ClassId).ToList();
      enrolledCoursesList = enrollments.Select(x => x.CourseId).ToList();
    }

    var coursesRequest =
      _coursesGrpcService.ListCoursesAsync(query.ToGrpcGetEnrollmentsByCoursesRequest(enrolledClassesList, isAdmin),
        cancellationToken: ct);

    var coursesResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);

    if (coursesResult.IsError)
    {
      return coursesResult.FirstError;
    }

    var courses = coursesResult.Value;
    return courses.ToCourseListItemResponse(enrolledCoursesList);
  }
}
