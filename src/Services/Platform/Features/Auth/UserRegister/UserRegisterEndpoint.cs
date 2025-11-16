using Contracts.Users.Requests;

using FastEndpoints;

using Library.Auth;

using MassTransit;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Common.Database;
using Platform.Common.Database.Entities;
using Platform.Common.Services.JWT;

namespace Platform.Features.Auth.UserRegister;

public class UserRegisterEndpoint : Endpoint<UserRegisterRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly ApplicationDbContext _db;
  private readonly IJwtService _jwtService;
  private readonly ILogger<UserRegisterEndpoint> _logger;
  private readonly IPublishEndpoint _publishEndpoint;
  private readonly UserManager<ApplicationUser> _userManager;

  public UserRegisterEndpoint(UserManager<ApplicationUser> signInManager, ILogger<UserRegisterEndpoint> logger,
    IJwtService jwtService, IPublishEndpoint publishEndpoint, ApplicationDbContext db)
  {
    _userManager = signInManager;
    _logger = logger;
    _jwtService = jwtService;
    _publishEndpoint = publishEndpoint;
    _db = db;
  }

  public override void Configure()
  {
    Post("/api/auth/register");
    AllowAnonymous();
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserRegisterRequest request,
    CancellationToken ct)
  {
    var existingUser = await _userManager.FindByNameAsync(request.Email);
    if (existingUser != null)
    {
      _logger.LogWarning("User with {Email} already exists", request.Email);
      return Error.Conflict(description: "User already exists");
    }

    var result = await _userManager.CreateAsync(request.ToApplicationUser(), request.Password);

    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
      {
        _logger.LogWarning("Register failed: {Code} - {Description}", error.Code, error.Description);
      }

      return Error.Failure(description: "Register failed");
    }

    var user = await _userManager.FindByNameAsync(request.Email);
    if (user is null)
    {
      return Error.Failure(description: "User not found");
    }

    await _userManager.AddToRolesAsync(user, ["User"]);

    await _publishEndpoint.Publish(user.ToUserCreatedEvent(), ct);
    _logger.LogInformation("User {UserName} registered", user.UserName);
    var jwtTokenResponse = await _jwtService.GenerateJwtToken(user);

    var refreshToken = new RefreshToken
    {
      Token = Generators.GenerateToken(), ExpiresAt = DateTime.Now.AddDays(7), UserId = user.Id
    };

    await _db.RefreshTokens.AddAsync(refreshToken, ct);
    await _db.SaveChangesAsync(ct);
    return new AccessTokenResponse
    {
      AccessToken = jwtTokenResponse.AccessToken,
      ExpiresIn = jwtTokenResponse.ExpiresIn,
      RefreshToken = refreshToken.Token
    };
  }
}
