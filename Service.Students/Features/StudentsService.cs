using Grpc.Core;
using Library.GRPC;
using Service.Students.Features.DeleteStudent;
using Service.Students.Features.GetStudent;
using Service.Students.Features.ListStudent;
using StudentsGRPC;

namespace Service.Students.Features;

public class StudentsService : GrpcStudentsService.GrpcStudentsServiceBase
{
    private readonly ILogger<StudentsService> _logger;
    private readonly IMediator _mediator;

    public StudentsService(ILogger<StudentsService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task<GrpcStudentResponse> GetStudentById(GrpcGetStudentByIdRequest request,
        ServerCallContext context)
    {
        var studentResult = await _mediator.Send(new GetStudentQuery(request.Id));
        return studentResult.Match(
            GrpcToStudentMapper.StudentToGrpcStudentResponse,
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override Task<GrpcListStudentsResponse> ListStudents(GrpcListStudentsRequest request,
        ServerCallContext context)
    {
        var studentsResult = _mediator.Send(new ListStudentsQuery
        {
            PageNumber = request.Page,
            PageSize = request.PageSize
        });

        return studentsResult.Match(
            data => new GrpcListStudentsResponse
            {
                Items = { data.Items.Select(GrpcToStudentMapper.StudentToGrpcStudentResponse) },
                TotalCount = data.TotalCount,
                PageSize = data.PageSize,
                CurrentPage = data.CurrentPage,
                TotalPages = data.TotalPages,
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }

    public override Task<GrpcUpdatedResponse> DeleteStudent(GrpcDeleteStudentRequest request, ServerCallContext context)
    {
        var deleteStudentResult = _mediator.Send(new DeleteStudentCommand(request.Id));
        return deleteStudentResult.Match(
            _ => new GrpcUpdatedResponse
            {
                Updated = true,
                Message = "Student deleted successfully"
            },
            error => throw GrpcErrorHandler.ThrowAndLogRpcException(error, _logger));
    }
}