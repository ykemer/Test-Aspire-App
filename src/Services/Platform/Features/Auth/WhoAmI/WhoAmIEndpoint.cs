using Contracts.Users.Responses;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

using Platform.Common.Auth;
using Platform.Common.Database.Entities;

namespace Platform.Features.Auth.WhoAmI;

/// <summary>
/// Returns the profile of the signed-in user.
/// </summary>
public class WhoAmIEndpoint : EndpointWithoutRequest<ErrorOr<UserInfoResponse>>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public WhoAmIEndpoint(UserManager<ApplicationUser> userManager) => _userManager = userManager;

  public override void Configure()
  {
    Get("/api/auth/whoami");
    Policies(Common.Auth.Policies.Users);
    Description(x => x.WithTags("Auth"));
  }

  public override async Task<ErrorOr<UserInfoResponse>> ExecuteAsync(CancellationToken ct)
  {
    var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
    if (user is null)
    {
      // The token is valid but the account was deleted since it was issued.
      return Error.Unauthorized("platform.auth.user_not_found", "User not found.");
    }

    return new UserInfoResponse
    {
      Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email!
    };
  }
}
