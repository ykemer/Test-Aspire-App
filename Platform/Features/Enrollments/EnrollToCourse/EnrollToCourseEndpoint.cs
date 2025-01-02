using Platform.Services.User;
using Contracts.Courses.Requests;
using CoursesGRPCClient;
using EnrollmentsGRPCClient;
using FastEndpoints;
using Platform.Services.Middleware;
using StudentsGRPCClient;


namespace Platform.Features.Enrollments.EnrollToCourse;

public class EnrollToCourseEndpoint : Endpoint<ChangeCourseEnrollmentRequest, ErrorOr<Updated>>
{
    private readonly IUserService _userService;
    private readonly GrpcCoursesService.GrpcCoursesServiceClient _coursesGrpcService;
    private readonly GrpcEnrollmentsService.GrpcEnrollmentsServiceClient _enrollmentsGrpcService;
    private readonly IGrpcRequestMiddleware _grpcRequestMiddleware;
    private readonly GrpcStudentsService.GrpcStudentsServiceClient _studentsGrpcService;

    public EnrollToCourseEndpoint(
        IUserService userService,
        GrpcCoursesService.GrpcCoursesServiceClient coursesGrpcService,
        GrpcEnrollmentsService.GrpcEnrollmentsServiceClient enrollmentsGrpcService,
        IGrpcRequestMiddleware grpcRequestMiddleware, GrpcStudentsService.GrpcStudentsServiceClient studentsGrpcService)
    {
        _userService = userService;
        _coursesGrpcService = coursesGrpcService;
        _enrollmentsGrpcService = enrollmentsGrpcService;
        _grpcRequestMiddleware = grpcRequestMiddleware;
        _studentsGrpcService = studentsGrpcService;
    }

    public override void Configure()
    {
        Post("/api/courses/enroll");
        Policies("RequireUserRole");
        Claims("UserId");
    }

    public override async Task<ErrorOr<Updated>> ExecuteAsync(ChangeCourseEnrollmentRequest request,
        CancellationToken ct)
    {
        var userId = _userService.IsAdmin(User) ? request.StudentId : _userService.GetUserId(User);
        if (userId == Guid.Empty) return Error.Failure(description: "User not found");


        var studentRequest = _studentsGrpcService.GetStudentByIdAsync(new GrpcGetStudentByIdRequest
        {
            Id = userId.ToString()
        });
        var studentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(studentRequest, ct);
        if (studentResponse.IsError)
        {
            return studentResponse.Errors[0];
        }

        var student = studentResponse.Value;

        var courseRequest = _coursesGrpcService.GetCourseAsync(new GrpcGetCourseRequest
        {
            Id = request.CourseId.ToString()
        });

        var courseResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(courseRequest, ct);
        if (courseResponse.IsError)
        {
            return courseResponse.Errors[0];
        }


        var enrollmentRequest = _enrollmentsGrpcService.EnrollStudentAsync(new GrpcEnrollStudentRequest
        {
            CourseId = request.CourseId.ToString(),
            StudentId = userId.ToString(),
            StudentFirstName = student.FirstName,
            StudentLastName = student.LastName
        });

        var enrollmentResponse = await _grpcRequestMiddleware.SendGrpcRequestAsync(enrollmentRequest, ct);
        return enrollmentResponse.Match<ErrorOr<Updated>>(
            _ => Result.Updated,
            error => error
        );
    }
}