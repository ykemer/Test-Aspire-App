using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Auth.RefreshAccessToken;

public class RefreshTokenRequestValidator : Validator<RefreshAccessTokenRequest>
{
  public RefreshTokenRequestValidator() => RuleFor(x => x.RefreshToken).NotEmpty().MinimumLength(10);
}
