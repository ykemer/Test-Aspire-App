using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Auth.RefreshAccessToken;

public class RefreshTokenRequestValidator : Validator<RefreshAccessTokenRequest>
{
  public RefreshTokenRequestValidator() => RuleFor(x => x.RefreshToken)
    .NotNull()
    .NotEmpty()
    .MinimumLength(10)
    .WithMessage("RefreshToken can not be empty")
    .MaximumLength(100)
    .WithMessage("RefreshToken can not exceed 100 characters");
}
