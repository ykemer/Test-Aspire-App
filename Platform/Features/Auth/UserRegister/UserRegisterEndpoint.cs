using Contracts.Users.Events;
using Contracts.Users.Requests;
using Platform.Services.JWT;
using FastEndpoints;
using Library.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Platform.AsyncDataServices;

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

    public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserRegisterRequest req, CancellationToken ct)
    {
        var existingUser = await _userManager.FindByNameAsync(req.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("User with {Email} already exists", req.Email);
            return Error.Conflict(description: "User already exists");
        }


        var refreshToken = Generators.GenerateToken();
        var result = await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName,
            DateOfBirth = req.DateOfBirth
        }, req.Password);


        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                _logger.LogWarning("Register failed: {Code} - {Description}", error.Code, error.Description);
            return Error.Failure(description: "Register failed");
        }

        var user = await _userManager.FindByNameAsync(req.Email);
        await _userManager.AddToRolesAsync(user, ["User"]);


        var studentCreateCommand = new UserCreatedEvent
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email
        };

        _messageBusClient.PublishUserRegisteredMessage(studentCreateCommand);
        _logger.LogInformation("Register succeeded");
        var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

        return new AccessTokenResponse
        {
            AccessToken = jwtTokenResponse.AccessToken,
            ExpiresIn = jwtTokenResponse.ExpiresIn,
            RefreshToken = refreshToken
        };
    }
}