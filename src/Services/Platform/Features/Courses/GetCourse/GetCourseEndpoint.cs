using Contracts.Courses.Responses;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

namespace Platform.Features.Courses.GetCourse;

public class GetCourseEndpoint : EndpointWithoutRequest<ErrorOr<CourseResponse>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IUserService _userService;

  public GetCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
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
    Get("/api/courses/{CourseId}");
    Policies("RequireUserRole");
    Description(x => x.WithTags("Courses"));
  }


  [OutputCache(PolicyName = "CoursesCache")]
  public override async Task<ErrorOr<CourseResponse>> ExecuteAsync(
    CancellationToken ct)
  {
    var id = Route<Guid>("CourseId");


    var courseList = new List<string>();

    if (!_userService.IsAdmin(User))
    {
      var req = new GrpcGetStudentEnrollmentsRequest { StudentId = _userService.GetUserId(User).ToString() };

      var enrollmentsRequest =
        _enrollmentsGrpcService.GetStudentEnrollmentsAsync(req, cancellationToken: ct);

      var enrollmentsResult =
        await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentsRequest, ct);
      if (enrollmentsResult.IsError)
      {
        return enrollmentsResult.FirstError;
      }

      var enrollments = enrollmentsResult.Value.Items;
      courseList = enrollments.Select(x => x.CourseId).ToList();
    }

    var request =
      _coursesGrpcService.GetCourseAsync(
        new GrpcGetCourseRequest
        {
          Id = id.ToString(), EnrolledClasses = { courseList }, ShowAll = _userService.IsAdmin(User)
        }, cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    return result.Match<ErrorOr<CourseResponse>>(
      data => data.ToCourseResponse(),
      error => error);
  }
}
