using Contracts.Users.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Platform.Entities;

namespace Platform.Features.Auth.RefreshTokenRevoke;

public class RefreshTokenRevokeEndpoint : Endpoint<RefreshAccessTokenRequest, ErrorOr<Deleted>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenRevokeEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/auth/revoke");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Deleted>> ExecuteAsync(RefreshAccessTokenRequest req, CancellationToken ct)
    {
        var user = _userManager.Users.FirstOrDefault(i => i.RefreshToken == req.RefreshToken);
        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Error.Unauthorized(description: "Refresh token is not valid");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Result.Deleted;
    }
}