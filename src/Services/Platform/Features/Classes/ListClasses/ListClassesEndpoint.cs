using System.Security.Claims;

using ClassesGRPCClient;

using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

namespace Platform.Features.Classes.ListClasses;

public class ListClassesEndpoint : Endpoint<ListCoursesRequest, ErrorOr<PagedList<ClassListItemResponse>>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IUserService _userService;

  public ListClassesEndpoint(GrpcClassService.GrpcClassServiceClient classesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware, IUserService userService,
    GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService)
  {
    _classesGrpcService = classesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _userService = userService;
    _enrollmentsGrpcService = enrollmentsGrpcService;
  }

  public override void Configure()
  {
    Get("/api/courses/{CourseId}/classes");
    Policies("RequireUserRole");
    Claims("UserId");
    ResponseCache(60);
    Options(x => x.RequireRateLimiting("fixed-per-user"));
    Description(x => x.WithTags("Classes"));
  }

  [OutputCache(PolicyName = "CoursesCache")]
  public override async Task<ErrorOr<PagedList<ClassListItemResponse>>> ExecuteAsync(ListCoursesRequest query,
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");

    var enrolledClasses = new List<string>();

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
      enrolledClasses = enrollments.Select(x => x.ClassId).ToList();
    }

    var coursesRequest =
      _classesGrpcService.ListClassesAsync(query.ToGrpcListClassRequest(enrolledClasses, isAdmin, courseId.ToString()),
        cancellationToken: ct);

    var coursesResult =
      await _grpcRequestMiddleware.SendGrpcRequestAsync(coursesRequest, ct);

    if (coursesResult.IsError)
    {
      return coursesResult.FirstError;
    }

    var courses = coursesResult.Value;
    return courses.ToClassListItemResponse(enrolledClasses);
  }
}
