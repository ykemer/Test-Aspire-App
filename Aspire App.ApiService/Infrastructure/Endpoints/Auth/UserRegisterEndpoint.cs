﻿using Aspire_App.ApiService.Application.Students.Commands;
using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Infrastructure.Services;
using Contracts.Auth.Requests;
using FastEndpoints;
using Library.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

namespace Aspire_App.ApiService.Infrastructure.Endpoints.Auth;

public class UserRegisterEndpoint : Endpoint<UserRegisterRequest, IResult>
{
    private readonly JwtService _jwtService;
    private readonly ILogger<UserRegisterEndpoint> _logger;
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRegisterEndpoint(UserManager<ApplicationUser> signInManager, ILogger<UserRegisterEndpoint> logger,
        JwtService jwtService, IMediator mediator)
    {
        _userManager = signInManager;
        _logger = logger;
        _jwtService = jwtService;
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(UserRegisterRequest req, CancellationToken ct)
    {
        var existingUser = await _userManager.FindByNameAsync(req.Email);
        if (existingUser != null)
            return Results.Problem("User already exists", statusCode: StatusCodes.Status409Conflict);

        var refreshToken = Generators.GenerateToken();
        var result = await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth
        }, req.Password);


        if (!result.Succeeded) return Results.Problem("Register failed", statusCode: StatusCodes.Status400BadRequest);


        var user = await _userManager.FindByNameAsync(req.Email);
        await _mediator.Send(new StudentCreateCommand(Guid.Parse(user.Id), user.FirstName, user.LastName, user.Email,
            user.DateOfBirth));
        await _userManager.AddToRolesAsync(user, new[] { "User" });

        _logger.LogInformation("Register succeeded");
        var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

        return Results.Ok(new AccessTokenResponse
        {
            AccessToken = jwtTokenResponse.AccessToken,
            ExpiresIn = jwtTokenResponse.ExpiresIn,
            RefreshToken = refreshToken
        });
    }
}