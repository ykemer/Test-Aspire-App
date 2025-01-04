﻿using Contracts.Users.Requests;
using Platform.Services.JWT;
using FastEndpoints;
using Library.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Platform.AsyncDataServices;
using Platform.Entities;
using Platform.Middleware.Mappers;

namespace Platform.Features.Auth.UserRegister;

public class UserRegisterEndpoint : Endpoint<UserRegisterRequest, ErrorOr<AccessTokenResponse>>
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserRegisterEndpoint> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageBusClient _messageBusClient;

    public UserRegisterEndpoint(UserManager<ApplicationUser> signInManager, ILogger<UserRegisterEndpoint> logger,
        IJwtService jwtService, IMessageBusClient messageBusClient)
    {
        _userManager = signInManager;
        _logger = logger;
        _jwtService = jwtService;
        _messageBusClient = messageBusClient;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserRegisterRequest request, CancellationToken ct)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("User with {Email} already exists", request.Email);
            return Error.Conflict(description: "User already exists");
        }


        var refreshToken = Generators.GenerateToken();
        var result = await _userManager.CreateAsync(request.ToApplicationUser(), request.Password);


        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                _logger.LogWarning("Register failed: {Code} - {Description}", error.Code, error.Description);
            return Error.Failure(description: "Register failed");
        }

        var user = await _userManager.FindByNameAsync(request.Email);
        await _userManager.AddToRolesAsync(user, ["User"]);
        
        _messageBusClient.PublishUserRegisteredMessage(user.ToUserCreatedEvent());
        _logger.LogInformation("User {UserName} registered", user.UserName);
        var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

        return new AccessTokenResponse
        {
            AccessToken = jwtTokenResponse.AccessToken,
            ExpiresIn = jwtTokenResponse.ExpiresIn,
            RefreshToken = refreshToken
        };
    }
}