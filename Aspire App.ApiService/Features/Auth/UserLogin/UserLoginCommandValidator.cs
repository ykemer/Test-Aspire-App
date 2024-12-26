using Contracts.Auth.Requests;
using FastEndpoints;
using FluentValidation;

namespace Aspire_App.ApiService.Features.Auth.UserLogin;

public class UserLoginCommandValidator : Validator<UserLoginRequest>
{
    public UserLoginCommandValidator()
    {
        RuleFor(i => i.Email)
            .NotEmpty()
            .WithMessage("Email can not be empty");

        RuleFor(i => i.Password)
            .NotEmpty()
            .WithMessage("Password can not be empty");
    }
}