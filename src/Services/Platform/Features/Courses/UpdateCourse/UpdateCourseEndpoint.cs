﻿using Contracts.Courses.Requests;

using CoursesGRPCClient;

using FastEndpoints;

using Platform.Middleware.Grpc;
using Platform.Middleware.Mappers;

namespace Platform.Features.Courses.UpdateCourse;

public class UpdateCourseEndpoint : Endpoint<UpdateCourseRequest,
  ErrorOr<Updated>>
{
  private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;

  public UpdateCourseEndpoint(GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
    IGrpcRequestMiddleware grpcRequestMiddleware)
  {
    _coursesGrpcService = coursesGrpcService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
  }

  public override void Configure()
  {
    Post("/api/courses/update");
    Policies("RequireAdministratorRole");
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(UpdateCourseRequest updateCourseCommand,
    CancellationToken ct)
  {
    var request =
      _coursesGrpcService.UpdateCourseAsync(updateCourseCommand.ToGrpcUpdateCourseRequest(), cancellationToken: ct);

    var output = await _grpcRequestMiddleware.SendGrpcRequestAsync(request, ct);
    return output.Match<ErrorOr<Updated>>(
      _ => Result.Updated,
      error => error
    );
  }
}
