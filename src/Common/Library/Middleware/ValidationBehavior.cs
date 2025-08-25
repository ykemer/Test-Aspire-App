using ErrorOr;

using FluentValidation;

using Mediator;

namespace _Application._Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }


    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null) return await next(message, cancellationToken);

        var validationResult = await _validator.ValidateAsync(message, cancellationToken);

        if (validationResult.IsValid) return await next(message, cancellationToken);

        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                error.PropertyName,
                error.ErrorMessage));

        return (dynamic)errors;
    }
}
