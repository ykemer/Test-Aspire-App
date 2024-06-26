﻿using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Infrastructure.Services;
using Contracts.Auth.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Auth;

public class RefreshAccessTokenEndpoint : Endpoint<RefreshAccessTokenRequest, IResult>
{
    private readonly JwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshAccessTokenEndpoint(JwtService jwtService, UserManager<ApplicationUser> userManager)
    {
        _jwtService = jwtService;
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(RefreshAccessTokenRequest req, CancellationToken ct)
    {
        var user = _userManager.Users.FirstOrDefault(i => i.RefreshToken == req.RefreshToken);
        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Results.Problem("Refresh token is not valid", statusCode: StatusCodes.Status401Unauthorized);

        var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = jwtTokenResponse.AccessToken,
            ExpiresIn = jwtTokenResponse.ExpiresIn,
            RefreshToken = user.RefreshToken ?? ""
        };
        return Results.Ok(accessTokenResponse);
    }
}