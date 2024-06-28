using Contracts.Auth.Requests;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Infrastructure.Validators.Auth;

public class RefreshTokenRequestValidator : Validator<RefreshAccessTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MinimumLength(10);
    }
}