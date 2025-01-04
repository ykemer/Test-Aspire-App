using System.Collections.Concurrent;
using System.Linq.Expressions;
using FastEndpoints;
using FluentValidation.Results;

namespace Platform.Middleware.Responses;

sealed class ResponseMiddleware : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        if (context.HttpContext.ResponseStarted() || context.Response is not IErrorOr errorOr)
            return Task.CompletedTask;

        if (!errorOr.IsError)
            return context.HttpContext.Response.SendAsync(GetValueFromErrorOr(errorOr), cancellation: ct);

        if (errorOr.Errors?.All(e => e.Type == ErrorType.Validation) is true)
        {
            return context.HttpContext.Response.SendErrorsAsync(
                failures: [..errorOr.Errors.Select(e => new ValidationFailure(e.Code, e.Description))],
                cancellation: ct);
        }
        
        var problem = errorOr.Errors?.FirstOrDefault(e => e.Type != ErrorType.Validation);
        return context.HttpContext.Response.SendErrorsAsync(
            statusCode:GetStatusCode(problem?.Type),
            failures: [..errorOr.Errors.Select(e => new ValidationFailure(e.Code, e.Description))],
            cancellation: ct);
    }

    static int GetStatusCode(ErrorType? errorType) => errorType switch
    {
        ErrorType.Conflict => 409,
        ErrorType.NotFound => 404,
        ErrorType.Unauthorized => 401,
        ErrorType.Forbidden => 403,
        ErrorType.Failure => 503,
        _ => 500
    };
    
    //cached compiled expressions for reading ErrorOr<T>.Value property
    static readonly ConcurrentDictionary<Type, Func<object, object>> _valueAccessors = new();
    
    static object GetValueFromErrorOr(object errorOr)
    {
        ArgumentNullException.ThrowIfNull(errorOr);
        var tErrorOr = errorOr.GetType();

        if (!tErrorOr.IsGenericType || tErrorOr.GetGenericTypeDefinition() != typeof(ErrorOr<>))
            throw new InvalidOperationException("The provided object is not an instance of ErrorOr<>.");

        return _valueAccessors.GetOrAdd(tErrorOr, CreateValueAccessor)(errorOr);

        static Func<object, object> CreateValueAccessor(Type errorOrType)
        {
            var parameter = Expression.Parameter(typeof(object), "errorOr");

            return Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Property(
                            Expression.Convert(parameter, errorOrType),
                            "Value"),
                        typeof(object)),
                    parameter)
                .Compile();
        }
    }
}