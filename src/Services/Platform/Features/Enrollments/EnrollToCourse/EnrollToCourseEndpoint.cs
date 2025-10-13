﻿using ClassesGRPCClient;

using Contracts.Courses.Requests;
using Contracts.Courses.Requests.Enrollments;
using Contracts.Enrollments.Commands;
using Contracts.Enrollments.Events;

using CoursesGRPCClient;

using EnrollmentsGRPCClient;

using FastEndpoints;

using MassTransit;

using Platform.Common.Middleware.Grpc;
using Platform.Common.Services.User;

using StudentsGRPCClient;

namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{

  private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
  private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;
  private readonly IUserService _userService;

  private readonly ISendEndpointProvider _sendEndpointProvider;


  public EnrollToCourseEndpoint(
    IUserService userService,
    IGrpcRequestMiddleware grpcRequestMiddleware,
    GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService,
    ISendEndpointProvider sendEndpointProvider)
  {
    _userService = userService;
    _grpcRequestMiddleware = grpcRequestMiddleware;
    _studentsGrpcService = studentsGrpcService;
    _sendEndpointProvider = sendEndpointProvider;
  }

  public override void Configure()
  {
    Post("/api/courses/{CourseId}/classes/{ClassId}/enroll");
    Policies("RequireUserRole");
    Claims("UserId");
  }

  public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
    CancellationToken ct)
  {
    var courseId = Route<Guid>("CourseId");
    var classId = Route<Guid>("ClassId");

    var userId = _userService.IsAdmin(User) ? request.StudentId : _userService.GetUserId(User);
    if (userId == Guid.Empty)
    {
      return Error.Failure(description: "User not found");
    }


    var studentRequest =
      _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest { Id = userId.ToString() });
    var
      studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
    if (studentResponse.IsError)
    {
      return studentResponse.Errors[0];
    }

    var student = studentResponse.Value;

    var sendUri = new Uri("queue:create-enrollment-command");

    var endpoint = await _sendEndpointProvider.GetSendEndpoint(sendUri);

    await endpoint.Send(new CreateEnrollmentCommand
    {
      CourseId = courseId.ToString(),
      ClassId = classId.ToString(),
      StudentId = userId.ToString(),
      FirstName = student.FirstName,
      LastName = student.LastName
    });

    return Result.Updated;
  }
}
