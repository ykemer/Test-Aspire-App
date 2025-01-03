using EnrollmentsGRPC;
using Grpc.Core;
using Library.GRPC;
using Service.Enrollments.Features.Enrollments.EnrollStudentToCourse;
using Service.Enrollments.Features.Enrollments.GetCourseEnrollments;
using Service.Enrollments.Features.Enrollments.ListEnrollmentsByCourses;
using Service.Enrollments.Features.Enrollments.UnenrollStudentToCourse;

namespace Service.Enrollments.Features.Enrollments;

public class EnrollmentsService : GrpcEnrollmentsService.GrpcEnrollmentsServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EnrollmentsService> _logger;

    public EnrollmentsService(IMediator mediator, ILogger<EnrollmentsService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<GrpcListEnrollmentsResponse> GetCourseEnrollments(GrpcGetCourseEnrollmentsRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetCourseEnrollmentsRequest(request.CourseId));
        return result.Match(
            enrollments => new GrpcListEnrollmentsResponse
            {
                Items = { enrollments.Select(GrpcDataToEnrollmentMapper.EnrollmentToGrpcEnrollmentResponseMap) }
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<ListOfEnrollmentsByCoursesResponse> GetEnrollmentsByCourses(
        GrpcGetEnrollmentsByCoursesRequest request, ServerCallContext context)
    {
        var result =
            await _mediator.Send(new ListOfEnrollmentsByCoursesQuery(request.CourseIds.ToList(), request.StudentId));
        return result.Match(
            enrollments => new ListOfEnrollmentsByCoursesResponse
            {
                Items = { enrollments.Select(GrpcDataToEnrollmentMapper.EnrollmentToGrpcEnrollmentResponseMap) }
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }


    public override async Task<GrpcUpdateEnrollmentResponse> EnrollStudent(GrpcEnrollStudentRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new EnrollStudentToCourseCommand(request.CourseId, request.StudentId, request.StudentFirstName, request.StudentLastName));
        return result.Match(
            created => new GrpcUpdateEnrollmentResponse
            {
                Success = true,
                Message = "Student enrolled successfully"
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override async Task<GrpcUpdateEnrollmentResponse> DeleteEnrollment(GrpcDeleteEnrollmentRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new UnenrollStudentFromCourseCommand(request.CourseId, request.StudentId));
        return result.Match(
            created => new GrpcUpdateEnrollmentResponse
            {
                Success = true,
                Message = "Student enrolled successfully"
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }
}