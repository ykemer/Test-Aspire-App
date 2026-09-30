using Contracts.Users.Requests;

using FastEndpoints;

using FluentValidation;

namespace Platform.Features.Auth.RefreshAccessToken;

/// <summary>
/// Used by both "refresh" and "revoke" (same request type).
/// </summary>
public class RefreshTokenRequestValidator : Validator<RefreshAccessTokenRequest>
{
  public const int MaxTokenLength = 200;

  public RefreshTokenRequestValidator() =>
    RuleFor(request => request.RefreshToken)
      .NotEmpty().WithMessage("Refresh token is required.")
      .MaximumLength(MaxTokenLength).WithMessage($"Refresh token must not exceed {MaxTokenLength} characters.");
}
