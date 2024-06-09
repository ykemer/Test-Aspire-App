using Aspire_App.ApiService.Application.Students.Commands;
using Aspire_App.ApiService.Application.Students.Responses;
using FastEndpoints;
using MediatR;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Students;

public class CreateStudentEndpoint : Endpoint<StudentCreateCommand,
    StudentResponse>
{
    private readonly IMediator _mediator;

    public CreateStudentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/students");
        AllowAnonymous();
        Description(b => b
            .ProducesProblemDetails(400, "application/json+problem") //if using RFC errors 
            .ProducesProblemFE<InternalErrorResponse>(500));
    }

    public override async Task<StudentResponse> ExecuteAsync(StudentCreateCommand studentCreateCommand, CancellationToken cancellationToken)
    {
        var createdStudentOutput = await _mediator.Send(studentCreateCommand);
        return createdStudentOutput;
    }
}

