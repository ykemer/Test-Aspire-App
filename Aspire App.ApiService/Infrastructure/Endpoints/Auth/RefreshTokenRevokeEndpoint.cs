using Aspire_App.ApiService.Domain.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Auth;

public class RefreshTokenRevokeEndpoint : Endpoint<RefreshRequest, IResult>
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

    public override async Task<IResult> ExecuteAsync(RefreshRequest req, CancellationToken ct)
    {
        var user = _userManager.Users.FirstOrDefault(i => i.RefreshToken == req.RefreshToken);
        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Results.Problem("Refresh token is not valid", statusCode: StatusCodes.Status401Unauthorized);

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Results.Ok();
    }
}