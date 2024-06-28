using Aspire_App.ApiService.Domain.Models;
using Contracts.Auth.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Auth;

public class RefreshTokenRevokeEndpoint : Endpoint<RefreshAccessTokenRequest, IResult>
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

    public override async Task<IResult> ExecuteAsync(RefreshAccessTokenRequest req, CancellationToken ct)
    {
        var user = _userManager.Users.FirstOrDefault(i => i.RefreshToken == req.RefreshToken);
        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Results.Problem("Refresh token is not valid", statusCode: StatusCodes.Status401Unauthorized);

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Results.Ok();
    }
}