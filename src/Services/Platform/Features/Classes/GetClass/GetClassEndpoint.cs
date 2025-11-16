using ClassesGRPCClient;

using Contracts.Courses.Responses;

using EnrollmentsGRPCClient;

using FastEndpoints;

using Microsoft.AspNetCore.OutputCaching;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;
using Platform.Features.Classes.CreateClass;

namespace Platform.Features.Classes.GetClass;

public class GetClassEndpoint : EndpointWithoutRequest<ErrorOr<ClassResponse>>
{
  private readonly GrpcClassService.GrpcClassServiceClient _classesGrpcService;
  private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly IUserService _userService;

  public GetClassEndpoint(GrpcClassService.GrpcClassServiceClient classesGrpcService,
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
    Get("/api/courses/{CourseId}/classes/{ClassId}");
    Policies("RequireUserRole");

    Description(x => x.WithTags("Classes"));
  }


  [OutputCache(PolicyName = "CoursesCache")]
  public override async Task<ErrorOr<ClassResponse>> ExecuteAsync(
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");


    var classList = new List<string>();

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
      classList = enrollments.Select(x => x.CourseId).ToList();
    }

    var request =
      _classesGrpcService.GetClassAsync(
        new GrpcGetClassRequest
        {
          Id = classId.ToString(),
          EnrolledClasses = { classList },
          ShowAll = _userService.IsAdmin(User),
          CourseId = courseId.ToString()
        }, cancellationToken: ct);

    var result = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    return result.Match<ErrorOr<ClassResponse>>(
      data => data.MapToClassResponse(),
      error => error);
  }
}
