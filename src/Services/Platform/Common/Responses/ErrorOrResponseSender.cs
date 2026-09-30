using System.Collections.Concurrent;
using System.Linq.Expressions;

using FastEndpoints;

using FluentValidation.Results;

namespace Platform.Common.Responses;

/// <summary>
/// Endpoints return <c>ErrorOr&lt;T&gt;</c>. This runs after each such endpoint and sends the HTTP response:
/// the value with 200 on success, or the errors with the matching status code (see <see cref="ToStatusCode"/>).
/// </summary>
internal sealed class ErrorOrResponseSender : IGlobalPostProcessor
{
  // Compiled readers for ErrorOr<T>.Value, one per T, so reflection runs only once per type.
  private static readonly ConcurrentDictionary<Type, Func<object, object>> s_valueReaders = new();

  public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
  {
    if (context.HttpContext.ResponseStarted() || context.Response is not IErrorOr errorOr)
    {
      return Task.CompletedTask;
    }

    if (!errorOr.IsError)
    {
      return context.HttpContext.Response.SendAsync(ReadValue(errorOr), cancellation: ct);
    }

    var errors = errorOr.Errors ?? [];
    var failures = errors.Select(error => new ValidationFailure(error.Code, error.Description)).ToList();

    if (errors.All(error => error.Type == ErrorType.Validation))
    {
      return context.HttpContext.Response.SendErrorsAsync(failures, cancellation: ct);
    }

    var mainError = errors.First(error => error.Type != ErrorType.Validation);
    return context.HttpContext.Response.SendErrorsAsync(failures, ToStatusCode(mainError.Type), cancellation: ct);
  }

  public static int ToStatusCode(ErrorType errorType) =>
    errorType switch
    {
      ErrorType.Validation => StatusCodes.Status400BadRequest,
      ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
      ErrorType.Forbidden => StatusCodes.Status403Forbidden,
      ErrorType.NotFound => StatusCodes.Status404NotFound,
      ErrorType.Conflict => StatusCodes.Status409Conflict,
      ErrorType.Failure => StatusCodes.Status503ServiceUnavailable,
      _ => StatusCodes.Status500InternalServerError
    };

  private static object ReadValue(object errorOr) =>
    s_valueReaders.GetOrAdd(errorOr.GetType(), CreateValueReader)(errorOr);

  private static Func<object, object> CreateValueReader(Type errorOrType)
  {
    var parameter = Expression.Parameter(typeof(object), "errorOr");
    var readValue = Expression.Convert(
      Expression.Property(Expression.Convert(parameter, errorOrType), nameof(ErrorOr<object>.Value)),
      typeof(object));
    return Expression.Lambda<Func<object, object>>(readValue, parameter).Compile();
  }
}
