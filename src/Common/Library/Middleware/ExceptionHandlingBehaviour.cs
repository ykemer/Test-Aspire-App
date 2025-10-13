using ErrorOr;

using Mediator;

using Microsoft.Extensions.Logging;

namespace Library.Middleware;

public class ExceptionHandlingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  private readonly ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> _logger;

  public ExceptionHandlingBehaviour(ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> logger)
  {
    _logger = logger;
  }

  public async ValueTask<TResponse> Handle(
    TRequest message,
    MessageHandlerDelegate<TRequest, TResponse> next,
    CancellationToken cancellationToken)
  {
    if (!typeof(IErrorOr).IsAssignableFrom(typeof(TResponse)))
    {
      return await next(message, cancellationToken);
    }

    try
    {
      return await next(message, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An unhandled exception occurred during the processing of request {RequestName}",
        typeof(TRequest).Name);

      var error = Error.Failure(
        code: "General.InternalServerError",
        description: $"An unexpected error occurred while processing {typeof(TRequest).Name}."
      );

      var errors = new List<Error> { error };


      var responseType = typeof(TResponse);

      if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ErrorOr<>))
      {
        var genericArgument = responseType.GetGenericArguments()[0];
        var errorOrGenericType = typeof(ErrorOr<>).MakeGenericType(genericArgument);


        var fromMethod = errorOrGenericType.GetMethod(
          "From",
          System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
          null,
          new[] { typeof(IReadOnlyList<Error>) },
          null);

        if (fromMethod is null)
        {
          _logger.LogWarning("Failed to find ErrorOr.From method for {Type}", responseType.Name);
          throw;
        }

        var errorOrInstance = fromMethod.Invoke(null, new object[] { errors });


        return (TResponse)errorOrInstance!;
      }


      if (typeof(TResponse) == typeof(IReadOnlyList<Error>))
      {
        return (TResponse)(object)errors;
      }


      throw;
    }
  }
}
