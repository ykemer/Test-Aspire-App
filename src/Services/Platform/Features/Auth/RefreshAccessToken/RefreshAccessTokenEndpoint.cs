using Contracts.Users.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.BearerToken;

using Platform.Common.Auth;

namespace Platform.Features.Auth.RefreshAccessToken;

/// <summary>
/// Trades a refresh token for a new access token and a new refresh token. The old refresh token stops working.
/// </summary>
public class RefreshAccessTokenEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly IAuthTokenService _authTokens;

  public RefreshAccessTokenEndpoint(IAuthTokenService authTokens) => _authTokens = authTokens;

  public override void Configure()
  {
    Post("/api/auth/refresh");
    AllowAnonymous();
    Description(x => x.WithTags("Auth"));
  }

  public override Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(RefreshAccessTokenRequest request,
    CancellationToken ct) =>
    _authTokens.RefreshAsync(request.RefreshToken!, ct);
}
