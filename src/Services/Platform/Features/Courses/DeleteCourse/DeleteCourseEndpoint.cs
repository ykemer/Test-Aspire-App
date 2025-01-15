using Contracts.Courses.Requests;

using CoursesGRPCClient;

using FastEndpoints;

using Grpc.Core;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.DeleteCourse;

public class DeleteCourseEndpoint : Endpoint<DeleteCourseRequest,
  ErrorOr<Deleted>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public DeleteCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Post("/api/courses/delete");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Deleted>> ExecuteAsync(DeleteCourseRequest deleteCourseCommand,
    CancellationToken ct)
  {
    AsyncUnaryCall<GrpcUpdatedCourseResponse>? request =
      _coursesGrpcService.DeleteCourseAsync(deleteCourseCommand.ToGrpcDeleteCourseRequest(), cancellationToken: ct);

    ErrorOr<GrpcUpdatedCourseResponse> output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    return output.Match<ErrorOr<Deleted>>(
      _ => Result.Deleted,
      error => error
    );
  }
}
