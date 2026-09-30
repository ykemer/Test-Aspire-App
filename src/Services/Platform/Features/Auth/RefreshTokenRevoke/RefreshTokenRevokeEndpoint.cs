using Contracts.Users.Requests;

using FastEndpoints;

using Platform.Common.Auth;

namespace Platform.Features.Auth.RefreshTokenRevoke;

/// <summary>
/// Signs out: the given refresh token stops working. Knowing the token is proof enough to revoke it.
/// </summary>
public class RefreshTokenRevokeEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<Deleted>>
{
  private readonly IAuthTokenService _authTokens;

  public RefreshTokenRevokeEndpoint(IAuthTokenService authTokens) => _authTokens = authTokens;

  public override void Configure()
  {
    Post("/api/auth/revoke");
    AllowAnonymous();
    Description(x => x.WithTags("Auth"));
  }

  public override Task<ErrorOr<Deleted>> ExecuteAsync(RefreshAccessTokenRequest request, CancellationToken ct) =>
    _authTokens.RevokeAsync(request.RefreshToken!, ct);
}
