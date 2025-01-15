﻿using ErrorOr;

using MediatR;
using MediatR.Pipeline;

using Microsoft.Extensions.Logging;

namespace Library.Middleware;

public class LoggingBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
  where TResponse : IErrorOr
{
  private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

  public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) => _logger = logger;


  public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Request: {Name}. {Request}", typeof(TRequest).Name, request);
    if (!response.IsError)
    {
      return Task.CompletedTask;
    }

    List<Error>? nonValidationErrors = response.Errors.Where(i => i.Type != ErrorType.Validation).ToList();
    foreach (Error error in nonValidationErrors)
    {
      _logger.LogError("Request: {Name}. Error: {@Error}", typeof(TRequest).Name, error);
    }

    return Task.CompletedTask;
  }
}
