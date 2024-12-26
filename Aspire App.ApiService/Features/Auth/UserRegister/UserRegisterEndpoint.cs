using Aspire_App.ApiService.Features.Students;
using Aspire_App.ApiService.Persistence;
using Aspire_App.ApiService.Services.JWT;
using Contracts.Auth.Requests;
using FastEndpoints;
using Library.Auth;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

namespace Aspire_App.ApiService.Features.Auth.UserRegister;

public class UserRegisterEndpoint : Endpoint<UserRegisterRequest, ErrorOr<AccessTokenResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserRegisterEndpoint> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRegisterEndpoint(UserManager<ApplicationUser> signInManager, ILogger<UserRegisterEndpoint> logger,
        IJwtService jwtService, ApplicationDbContext dbContext)
    {
        _userManager = signInManager;
        _logger = logger;
        _jwtService = jwtService;
        _dbContext = dbContext;
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

        await _dbContext.Students.AddAsync(new Student
        {
            Id = Guid.Parse(user.Id),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth
        }, ct);

        await _dbContext.SaveChangesAsync(ct);


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