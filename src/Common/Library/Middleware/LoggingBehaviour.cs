using ErrorOr;

using Mediator;

using Microsoft.Extensions.Logging;

namespace Library.Middleware;

/// <summary>
/// Logs which request was handled and how it ended.
/// The request content is never logged: it can contain personal data (names, emails, ids).
/// </summary>
public class LoggingBehaviour<TRequest, TResponse> : MessagePostProcessor<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
  where TResponse : IErrorOr
{
  private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

  public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) => _logger = logger;

  protected override ValueTask Handle(TRequest request, TResponse response, CancellationToken cancellationToken)
  {
    var requestName = typeof(TRequest).Name;

    if (!response.IsError)
    {
      _logger.LogInformation("Handled {RequestName}", requestName);
      return ValueTask.CompletedTask;
    }

    foreach (var error in response.Errors ?? [])
    {
      var level = error.Type is ErrorType.Failure or ErrorType.Unexpected ? LogLevel.Error : LogLevel.Warning;
      _logger.Log(level, "{RequestName} failed: {ErrorType} {ErrorCode}", requestName, error.Type, error.Code);
    }

    return ValueTask.CompletedTask;
  }
}
