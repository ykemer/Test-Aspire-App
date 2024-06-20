using Aspire_App.ApiService.Application.Auth.Commmands;
using Aspire_App.ApiService.Application.Students.Commands;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Application.Students.Validators;

public class RefreshTokenRequestValidator : Validator<RefreshRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().MinimumLength(10);
    }
}