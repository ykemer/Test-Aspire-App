using MediatR.Pipeline;

namespace Service.Enrollments.Middleware;

public class LoggingBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }


    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (!response.IsError) return Task.CompletedTask;

        var nonValidationErrors = response.Errors.Where(i => i.Type != ErrorType.Validation).ToList();
        foreach (var error in nonValidationErrors)
        {
            _logger.LogError("Request: {Name}. Error: {@Error}", typeof(TRequest).Name, error);
        }

        return Task.CompletedTask;
    }
}