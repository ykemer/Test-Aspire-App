using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Validators.Auth;

public class RefreshTokenRequestValidator : Validator<RefreshRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MinimumLength(10);
    }
}