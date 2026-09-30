using Contracts.Users.Requests;

using FastEndpoints;

using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

using Rebus.Bus;

namespace Platform.Features.Auth.UserRegister;

/// <summary>
/// Creates a new user with the "User" role, tells the other services about it (the Students service
/// creates the student from the "user created" event) and signs the user in.
/// </summary>
public class UserRegisterEndpoint : Endpoint<UserRegisterRequest, ErrorOr<AccessTokenResponse>>
{
  private readonly IAuthTokenService _authTokens;
  private readonly IBus _bus;
  private readonly ILogger<UserRegisterEndpoint> _logger;
  private readonly UserManager<ApplicationUser> _userManager;

  public UserRegisterEndpoint(UserManager<ApplicationUser> userManager, IAuthTokenService authTokens, IBus bus,
    ILogger<UserRegisterEndpoint> logger)
  {
    _userManager = userManager;
    _authTokens = authTokens;
    _bus = bus;
    _logger = logger;
  }

  public override void Configure()
  {
    Post("/api/auth/register");
    AllowAnonymous();
    Description(x => x.WithTags("Auth"));
  }

  public override async Task<ErrorOr<AccessTokenResponse>> ExecuteAsync(UserRegisterRequest request,
    CancellationToken ct)
  {
    var user = request.ToApplicationUser();

    var created = await _userManager.CreateAsync(user, request.Password);
    if (!created.Succeeded)
    {
      return created.Errors.ToApiErrors();
    }

    var roleAdded = await _userManager.AddToRoleAsync(user, Common.Auth.Roles.User);
    if (!roleAdded.Succeeded)
    {
      // Only happens if the roles were never seeded: a setup problem, not a user mistake.
      throw new InvalidOperationException($"Could not give the new user the '{Common.Auth.Roles.User}' role.");
    }

    // Note: saving the user and publishing the event are two separate steps. If publishing fails,
    // the user exists without a student record. A transactional outbox would close this gap.
    await _bus.Publish(user.ToUserCreatedEvent());
    _logger.LogInformation("User {UserId} registered", user.Id);

    return await _authTokens.IssueTokensAsync(user, ct);
  }
}
